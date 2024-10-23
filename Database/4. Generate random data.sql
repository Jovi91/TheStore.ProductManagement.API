
DECLARE @ProductNames Table([Name] nvarchar(50))
INSERT INTO @ProductNames([Name])
VALUES ('Laptop')
		,('Smartwatch ')
		,('Smartphone')
		,('Headphones')
		,('TV UltraHD')
		,('Gaming Consol')
		,('Tablet')
		,('Drone')
		,('Monitor');


DECLARE @ProductDescriptions Table([Description] nvarchar(200))
INSERT INTO @ProductDescriptions([Description])
VALUES ('High-performance device')
		,('Model with good battery life')
		,('Stylish design')
		,('Powerful processor')
		,('Advanced features included')
		,('Next-generation features with enhanced security')
		,('Lightweight and portable, perfect for travel')
		,('Water-resistant')
		,('Compatible with most devices via Bluetooth')

DECLARE @ProductBrands Table([Brand] nvarchar(50))
INSERT INTO @ProductBrands([Brand])
VALUES ('Samsung')
		,('Xiaomi')
		,('Apple')
		,('Panasonic')
		,('LG')

DECLARE @Currencies Table(Code char(3), Multiplier decimal(7,4))
INSERT INTO @Currencies(Code, Multiplier)
VALUES ('PLN', 1.00)
		,('EUR', 4.2955)
		,('USD', 3.9468)
		,('DKK', 0.5757)


------------------------------- GENERATE -----------------------------------------

DECLARE @i int = 0
DECLARE @numOfProducts int = 1000
DECLARE @numOfCurrencies int = (Select count(1) FROM @Currencies)

DECLARE @productFullName nvarchar(60)
DECLARE @productName nvarchar(50)
DECLARE @productModel nvarchar(10)
DECLARE @productDescription nvarchar(255)
DECLARE @productBrand nvarchar(50)
DECLARE @productId int
DECLARE @PricePln decimal(10,2)

WHILE(@i < @numOfProducts)
BEGIN

    SET @productName = (SELECT TOP 1 [Name] FROM @ProductNames ORDER BY NEWID()) 
    SET @productModel = (SELECT CHAR(65 + ABS(CHECKSUM(NEWID())) % 26) + CAST(CAST((RAND() * 1000) + 1 AS INT) AS CHAR(3)))
    SET @productBrand = (SELECT TOP 1 Brand FROM @ProductBrands ORDER BY NEWID());
    SET @productFullName = CONCAT(@productName, ' ', @productModel)
    SET @productDescription = (SELECT TOP 1 [Description] FROM @ProductDescriptions ORDER BY NEWID())


    IF NOT EXISTS (SELECT 1 FROM [dbo].[Product] WHERE ProductName = @productFullName)
    BEGIN
        INSERT INTO [dbo].[Product] (ProductName, ProductDescription, ProductBrand)
        VALUES (@productFullName, @productDescription, @productBrand)

        SET @productId = SCOPE_IDENTITY()
        SET @pricePln = CAST((RAND(CHECKSUM(NEWID())) * 1000 + 100) AS DECIMAL(10, 2))

        INSERT INTO Price (ProductId, Currency, Price, StartDate, EndDate)
        SELECT 
            @productId,
            Code,
            @pricePln / Multiplier,
            DATEADD(day, CAST((RAND() * -1000) - 1 AS INT), GETDATE()),
            DATEADD(day, CAST((RAND() * 1000) + 1 AS INT), GETDATE())
        FROM 
            @Currencies 

		SET @i = @i + 1
    END

    
END


/*
SELECT * 
FROM 
	[Product] pr
JOIN 
	[Price] p on pr.ProductId=p.ProductId
*/


