using Insight.Database;
using SqlInsightDbProvider = Insight.Database.Providers.MsSqlClient.SqlInsightDbProvider;
using System.Data.SqlClient;
using System.Text.Json;
using TheStore.ProductManagement.API.Models;
namespace TheStore.ProductManagement.API.Database;

public sealed class DatabaseService : IDatabaseService
{
    private readonly SqlConnection _db;
    public DatabaseService(IConfiguration configuration)
    {
        SqlInsightDbProvider.RegisterProvider();
        _db = new(configuration.GetConnectionString("TheStoreDb"));

    }

    public async Task<DbResults<Product[]?>> GetProductDataFromDb(string? productName, int? id)
    {
        Product[]? product = null;
        var i = _db.As<IDataRepository>();
        var outputParams = new IDataRepository.OutputParamsForGet(0, string.Empty, string.Empty);

        await i.GetProductData(productName, id, outputParams);

        if (outputParams.StatusId == 200)
            product = JsonSerializer.Deserialize<Product[]>(outputParams.ProductDetails);

        return new(product, outputParams.StatusId, outputParams.Message);
    }

    public async Task<DbResults<Product[]?>> GetAllProductDataFromDb(int? pageNumber, int? pageSize)
    {
        Product[]? product = null;
        var i = _db.As<IDataRepository>();
        var outputParams = new IDataRepository.OutputParamsForGet(0, string.Empty, string.Empty);

        var startRow = (pageNumber - 1) * pageSize;
        await i.GetAllProductData(startRow, pageSize, outputParams); 

        if (outputParams.StatusId == 200)
            product = JsonSerializer.Deserialize<Product[]>(outputParams.ProductDetails);

        return new(product, outputParams.StatusId, outputParams.Message);
    }


    public async Task<DbResults<string>> AddProductDataIntoDb(Product product)
    {
        var i = _db.As<IDataRepository>();
        var outputParams = new IDataRepository.OutputParamsForPut(0, string.Empty, 0);

        var productJson = JsonSerializer.Serialize(product);

        await i.AddProductData(productJson, outputParams); 

        return new($"ProductId: {outputParams.ProductId}", outputParams.StatusId, outputParams.Message);
    }

    public async Task ApiErrorSaveToDb(ApiErrorLog errorLog)
    {
        var i = _db.As<IDataRepository>();


        var errorLogJson = JsonSerializer.Serialize(errorLog);

        await i.ApiErrorsLog(errorLogJson); 
    }


}
