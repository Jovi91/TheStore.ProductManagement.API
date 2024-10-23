/*
Run this script on:

        (local)\SQLEXPRESS.THESTORE

*/
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL Serializable
GO
BEGIN TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [logs].[ProcedureLogInsert]'
GO



-- =============================================
-- Author:		Jowita Olszta
-- Create date: 22.10.2024
-- Description:	Logging to the procedure responsible for storing procedure execution logs
-- =============================================
CREATE PROCEDURE [logs].[ProcedureLogInsert]
    @ProcedureName		NVARCHAR(255),
    @Parameters			NVARCHAR(MAX) = NULL,
    @ExecutionStatus	NVARCHAR(50),
    @ErrorMessage		NVARCHAR(MAX) = NULL,
    @Duration			INT = NULL,
	@OutputParams		NVARCHAR(MAX) = NULL
AS
BEGIN
    -- Insert log entry into ProcedureLogs
    INSERT INTO logs.ProcedureExecution (
        ProcedureName,
        Parameters,
        ExecutionStatus,
        ErrorMessage,
        Duration,
		OutputParams
    )
    VALUES (
        @ProcedureName,
        @Parameters,
        @ExecutionStatus,
        @ErrorMessage,
        @Duration,
		@OutputParams
    );
END;
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [api].[ProductDetailsGetAll]'
GO

-- =============================================
-- Author:		Jowita Olszta
-- Create date: 20.10.2024
-- Description:	Retrieving all products data via API .
-- =============================================
CREATE PROCEDURE [api].[ProductDetailsGetAll]
@StartRow		INT NULL,
@PageSize		INT NULL,
@ProductDetails NVARCHAR(max) OUTPUT,
@StatusId		int OUTPUT,
@Message		NVARCHAR(100) OUTPUT
AS BEGIN

-----------------------------------
--PARAMETERS FOR LOGGING
-----------------------------------
DECLARE
	@Procedure		VARCHAR(100)
	,@Parameters	VARCHAR(MAX)
	,@Start			DATETIME
	,@End			DATETIME
	,@ErrorMessage	VARCHAR(MAX)
	,@Duration		INT
	,@OutputParams	VARCHAR(MAX)


	BEGIN TRY

		SET @Procedure = OBJECT_NAME(@@procid)
		SET @Start = GETDATE()
		SET @Parameters = JSON_QUERY('{"StartRow": ' + ISNULL(CONVERT(NVARCHAR(10), @StartRow), 'null') + ', "PageSize": ' + ISNULL(CONVERT(NVARCHAR(10), @PageSize), 'null') + '}')

		
		;WITH Products AS (
			SELECT
				Product.ProductId,
				Product.ProductName,
				Product.ProductDescription,
				Product.ProductBrand
			FROM 
				dbo.Product 
			ORDER BY 
				ProductId
			OFFSET @StartRow ROWS
			FETCH NEXT @PageSize ROWS ONLY
		)
		SELECT @ProductDetails = (
			SELECT
				Product.ProductId,
				Product.ProductName,
				Product.ProductDescription,
				Product.ProductBrand,
				Prices.PriceId,
				Prices.Currency,
				Prices.Price,  -- Rename Price to Prices here
				Prices.StartDate,
				Prices.EndDate  
			FROM 
				Products AS Product
			JOIN
				dbo.Price AS Prices ON Product.ProductId = Prices.ProductId
			FOR JSON AUTO
		);


		IF(COUNT(@ProductDetails)<1)
		BEGIN
			SET @StatusId = 404;
			SET @Message = 'Products not found'
			Return;
		END

		SET @StatusId = 200;
		SET @Message = 'Ok'
		
	END TRY
	BEGIN CATCH

		SET @End= GETDATE()
		SET @StatusId = 500
		SET @Message = 'Databse returns an error. Error number: ' + CAST(ERROR_NUMBER() as varchar(10))
-----------------------------------
--LOGGING ERROR 
-----------------------------------
		SET @ErrorMessage = @Message + 
							' ERROR_LINE -> ' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'NULL') +
							' ERROR_NUMBER -> ' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'NULL') +
							' ERROR_MESSAGE -> ' + ISNULL(CAST(ERROR_MESSAGE() AS VARCHAR(500)), 'NULL')		
		SET @Duration = DATEDIFF(microsecond, @Start, @End) 
		SET @OutputParams = JSON_QUERY('{"Message": ' + ISNULL('"' + @Message + '"', 'null') +  ', "StatusId": ' + ISNULL(CAST(@StatusId AS NVARCHAR(10)), 'null') + '}');


		EXECUTE logs.ProcedureLogInsert
		   @ProcedureName = @Procedure
		  ,@Parameters = @Parameters
		  ,@ExecutionStatus = 'error'
		  ,@ErrorMessage = @ErrorMessage
		  ,@Duration =  @Duration
		  ,@OutputParams = @OutputParams

		SET @ProductDetails = NULL;
	END CATCH

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [api].[ProductApiErrorLog]'
GO



-- =============================================
-- Author:		Jowita Olszta
-- Create date: 22.10.2024
-- Description:	Logging errors returned by the product API
-- =============================================
CREATE PROCEDURE [api].[ProductApiErrorLog]
    @ApiErrorJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ProductApiErrorLogs (IpAddress, Endpoint, RequestTimestamp, RequestMethod, StatusCode, ErrorMessage)
    SELECT 
        IpAddress,
        Endpoint,
        TRY_CAST(RequestTimestamp AS DATETIME2) AS RequestTimestamp,
        RequestMethod,
        StatusCode,
        ErrorMessage
    FROM OPENJSON(@ApiErrorJson)
    WITH (
        IpAddress NVARCHAR(50) '$.IpAddress',
        Endpoint NVARCHAR(255) '$.Endpoint',
        RequestTimestamp NVARCHAR(50) '$.RequestTimestamp',
        RequestMethod NVARCHAR(10) '$.RequestMethod',
        StatusCode INT '$.StatusCode',
        ErrorMessage NVARCHAR(MAX) '$.Message' AS JSON
    )
END;
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [api].[ProductDetailsAdd]'
GO



-- =============================================
-- Author:		Jowita Olszta
-- Create date: 20.10.2024
-- Description:	Adding new products via API
-- =============================================
CREATE PROCEDURE [api].[ProductDetailsAdd]
@ProductJson	NVARCHAR(max),
@ProductId		INT OUTPUT,
@StatusId		INT OUTPUT,
@Message		NVARCHAR(100) OUTPUT
AS BEGIN
-----------------------------------
--PARAMETERS FOR LOGGING
-----------------------------------
DECLARE
	@Procedure			VARCHAR(100)
	,@Parameters		VARCHAR(MAX)
	,@Start				DATETIME
	,@End				DATETIME
	,@ErrorMessage		VARCHAR(MAX)
	,@Duration			INT
	,@OutputParams		VARCHAR(MAX)
	,@ExecutionStatus	VARCHAR(10)

	SET @Procedure = OBJECT_NAME(@@procid)
	SET @Start = GETDATE()
	SET @Parameters = JSON_QUERY('{"ProductJson":' + ISNULL(@ProductJson, 'null') + '}');


	BEGIN TRY
		
		IF EXISTS( SELECT 1 FROM Product WHERE ProductName = TRIM(JSON_VALUE(@ProductJson, '$.ProductName')))
		BEGIN
			SET @StatusId = 400;
			SET @Message = 'Product with that name allready exists'
			--SET @End= GETDATE()
			--SET @Duration = DATEDIFF(microsecond, @Start, @End) 
			SET @OutputParams = JSON_QUERY('{"Message": ' + ISNULL('"' + @Message + '"', 'null') +  ', "StatusId": ' + ISNULL(CAST(@StatusId AS NVARCHAR(10)), 'null') + '}');
			SET @ExecutionStatus = 'error'

			GOTO ENDLINE
		END

	BEGIN TRAN
		--Insert product details into Product table
		INSERT INTO Product (ProductName, ProductDescription, ProductBrand)
		SELECT 
			TRIM(ProductName),
			ProductDescription,
			ProductBrand
		FROM OPENJSON(@ProductJson)
		WITH (
			ProductName NVARCHAR(50),
			ProductDescription NVARCHAR(255),
			ProductBrand NVARCHAR(50)
		);

		SET @ProductId = SCOPE_IDENTITY();

		-- Insert into Price table
		INSERT INTO Price (ProductId, Currency, Price, StartDate, EndDate)
		SELECT 
			@ProductId,    
			Currency,
			Price,
			StartDate,
			EndDate
		FROM OPENJSON(@ProductJson, '$.Prices') 
		WITH (
			Currency CHAR(3),
			Price DECIMAL(10, 2),
			StartDate DATETIME2,
			EndDate DATETIME2
		);

	--RAISERROR('The value must be greater than zero.', 16, 1);
		SET @StatusId = 200;
		SET @Message = 'Product added successfully'

		SET @End= GETDATE()
		SET @Duration = DATEDIFF(microsecond, @Start, @End) 
		SET @OutputParams = JSON_QUERY('{"Message": ' + ISNULL('"' + @Message + '"', 'null') +  ', "StatusId": ' + ISNULL(CAST(@StatusId AS NVARCHAR(10)), 'null') + ', "ProductId": ' + ISNULL(CAST(@ProductId AS NVARCHAR(10)), 'null') +  '}');
		SET @ExecutionStatus = 'success'
	COMMIT TRAN
	END TRY
	BEGIN CATCH

		ROLLBACK TRAN
		SET @End= GETDATE()
		SET @StatusId = 500
		SET @Message = 'Databse returns an error. Error number: ' + CAST(ERROR_NUMBER() as varchar(10))


		SET @ErrorMessage = @Message + 
							' ERROR_LINE -> ' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'NULL') +
							' ERROR_NUMBER -> ' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'NULL') +
							' ERROR_MESSAGE -> ' + ISNULL(CAST(ERROR_MESSAGE() AS VARCHAR(500)), 'NULL')
 
		SET @Duration = DATEDIFF(microsecond, @Start, @End) 
		SET @OutputParams = JSON_QUERY('{"Message": ' + ISNULL('"' + @Message + '"', 'null') +  ', "StatusId": ' + ISNULL(CAST(@StatusId AS NVARCHAR(10)), 'null') + '}');
		SET @ExecutionStatus = 'error'
   
		SET @ProductId = NULL;

	END CATCH
ENDLINE:
-----------------------------------
--LOGGING
-----------------------------------
	
	EXECUTE logs.ProcedureLogInsert
		@ProcedureName = @Procedure
		,@Parameters = @Parameters
		,@ExecutionStatus = @ExecutionStatus
		,@ErrorMessage = @ErrorMessage
		,@Duration =  @Duration
		,@OutputParams = @OutputParams
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [api].[ProductDetailsGet]'
GO



-- =============================================
-- Author:		Jowita Olszta
-- Create date: 20.10.2024
-- Description:	Retrieving product data via API based on ProductId / based on ProductName .
-- =============================================
CREATE PROCEDURE [api].[ProductDetailsGet]
@ProductName	NVARCHAR(50) NULL,
@ProductId		INT NULL,
@ProductDetails NVARCHAR(max) OUTPUT,
@StatusId		int OUTPUT,
@Message		NVARCHAR(100) OUTPUT
AS BEGIN

-----------------------------------
--PARAMETERS FOR LOGGING
-----------------------------------
DECLARE
	@Procedure		VARCHAR(100)
	,@Parameters	VARCHAR(MAX)
	,@Start			DATETIME
	,@End			DATETIME
	,@ErrorMessage	VARCHAR(MAX)
	,@Duration		INT
	,@OutputParams	VARCHAR(MAX)


	BEGIN TRY

		SET @Procedure = OBJECT_NAME(@@procid)
		SET @Start = GETDATE()
		SET @Parameters = JSON_QUERY('{"ProductName": ' + ISNULL( '"' + @ProductName + '"'  , 'null') + ', "ProductId": ' + ISNULL(CONVERT(NVARCHAR(10), @ProductId), 'null') + '}')

		IF(@ProductName IS NULL AND @ProductId IS NULL)
		BEGIN
			SET @StatusId = 400;
			SET @Message = 'ProductId or ProductName is required.'
			Return;
		END

		SET @ProductDetails
		=
		(SELECT
			Product.ProductId,
			Product.ProductName,
			Product.ProductDescription,
			Product.ProductBrand,
			Prices.PriceId ,
			Prices.Currency ,
			Prices.Price  ,  -- Rename Price to Prices here
			Prices.StartDate  ,
			Prices.EndDate  
		FROM 
			dbo.Product 
		JOIN
			dbo.Price Prices ON Product.ProductId = Prices.ProductId 
				WHERE
				(Product.ProductId = @ProductId OR Product.ProductName = @ProductName)
		FOR JSON AUTO)

		IF(COUNT(@ProductDetails)<1)
		BEGIN
			SET @StatusId = 404;
			SET @Message = 'Product not found'
			Return;
		END

		SET @StatusId = 200;
		SET @Message = 'Ok'
		
	END TRY
	BEGIN CATCH

		SET @End= GETDATE()
		SET @StatusId = 500
		SET @Message = 'Databse returns an error. Error number: ' + CAST(ERROR_NUMBER() as varchar(10))
-----------------------------------
--LOGGING ERROR 
-----------------------------------
		SET @ErrorMessage = @Message + 
							' ERROR_LINE -> ' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'NULL') +
							' ERROR_NUMBER -> ' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'NULL') +
							' ERROR_MESSAGE -> ' + ISNULL(CAST(ERROR_MESSAGE() AS VARCHAR(500)), 'NULL')		
		SET @Duration = DATEDIFF(microsecond, @Start, @End) 
		SET @OutputParams = JSON_QUERY('{"Message": ' + ISNULL('"' + @Message + '"', 'null') +  ', "StatusId": ' + ISNULL(CAST(@StatusId AS NVARCHAR(10)), 'null') + '}');


		EXECUTE logs.ProcedureLogInsert
		   @ProcedureName = @Procedure
		  ,@Parameters = @Parameters
		  ,@ExecutionStatus = 'error'
		  ,@ErrorMessage = @ErrorMessage
		  ,@Duration =  @Duration
		  ,@OutputParams = @OutputParams

		SET @ProductDetails = NULL;
	END CATCH

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
COMMIT TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
-- This statement writes to the SQL Server Log so SQL Monitor can show this deployment.
IF HAS_PERMS_BY_NAME(N'sys.xp_logevent', N'OBJECT', N'EXECUTE') = 1
BEGIN
    DECLARE @databaseName AS nvarchar(2048), @eventMessage AS nvarchar(2048)
    SET @databaseName = REPLACE(REPLACE(DB_NAME(), N'\', N'\\'), N'"', N'\"')
    SET @eventMessage = N'Redgate SQL Compare: { "deployment": { "description": "Redgate SQL Compare deployed to ' + @databaseName + N'", "database": "' + @databaseName + N'" }}'
    EXECUTE sys.xp_logevent 55000, @eventMessage
END
GO
DECLARE @Success AS BIT
SET @Success = 1
SET NOEXEC OFF
IF (@Success = 1) PRINT 'The database update succeeded'
ELSE BEGIN
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
	PRINT 'The database update failed'
END
GO
