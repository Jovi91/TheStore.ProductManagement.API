using Microsoft.AspNetCore.Server.Kestrel.Transport.Quic;
using Insight.Database;
using SqlInsightDbProvider = Insight.Database.Providers.MsSqlClient.SqlInsightDbProvider;
using System.Data.SqlClient;
using TheStore.ProductManagement.API.Model;
using System.Text.Json;
using static TheStore.ProductManagement.API.IDataRepository;
using Insight.Database.MissingExtensions;
using TheStore.ProductManagement.API.Models;
namespace TheStore.ProductManagement.API
{
    public sealed class ProductService: IProductService
    {
        private readonly SqlConnection _db;
        public ProductService(IConfiguration configuration)
        {
            SqlInsightDbProvider.RegisterProvider();
            _db = new(configuration.GetConnectionString("TheStoreDb"));

        }

        public async Task<DbResults<Product[]>> GetProductDataFromDb(string? productName, int? id)
        {
            Product[] product = null;
            var i = _db.As<IDataRepository>();
            var outputParams = new IDataRepository.OutputParamsForGet(0, string.Empty, string.Empty);


            await i.GetProductData(productName, id, outputParams); //is insighdb methods async?


            //var tekst = "{\"ProductId\":201,\"Name\":\"Gaming Consol G402\",\"Description\":\"Stylish design\",\"ProductBrand\":\"Panasonic\",\"pr\":[{\"PriceId\":401,\"Currency\":\"PLN\",\"Price\":563.17,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"},{\"PriceId\":402,\"Currency\":\"EUR\",\"Price\":131.11,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"},{\"PriceId\":403,\"Currency\":\"USD\",\"Price\":142.69,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"},{\"PriceId\":404,\"Currency\":\"DKK\",\"Price\":978.24,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"}]}";
            //var tekst = "{\"ProductId\":201,\"ProductName\":\"Gaming Consol G402\",\"ProductDecription\":\"Stylish design\",\"ProductBrand\":\"Panasonic\",\"PriceId\":401,\"Currency\":\"PLN\",\"Price\":563.17,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"},{\"PriceId\":402,\"Currency\":\"EUR\",\"Price\":131.11,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"},{\"PriceId\":403,\"Currency\":\"USD\",\"Price\":142.69,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"},{\"PriceId\":404,\"Currency\":\"DKK\",\"Price\":978.24,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"}";      
            //var test2 = JsonSerializer.Deserialize<Product>(tekst);

            if(outputParams.StatusId == 200)
                product = JsonSerializer.Deserialize<Product[]>(outputParams.ProductDetails);

            return new (product, outputParams.StatusId, outputParams.Message);
        }


        public async Task<DbResults<string>> AddProductDataIntoDb(Product product)
        {
            var i = _db.As<IDataRepository>();
            var outputParams = new IDataRepository.OutputParamsForPut(0, string.Empty, 0);

            var productJson = JsonSerializer.Serialize(product);

            await i.AddProductData(productJson, outputParams); //is insighdb methods async?


            //var tekst = "{\"ProductId\":201,\"Name\":\"Gaming Consol G402\",\"Description\":\"Stylish design\",\"ProductBrand\":\"Panasonic\",\"pr\":[{\"PriceId\":401,\"Currency\":\"PLN\",\"Price\":563.17,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"},{\"PriceId\":402,\"Currency\":\"EUR\",\"Price\":131.11,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"},{\"PriceId\":403,\"Currency\":\"USD\",\"Price\":142.69,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"},{\"PriceId\":404,\"Currency\":\"DKK\",\"Price\":978.24,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"}]}";
            //var tekst = "{\"ProductId\":201,\"ProductName\":\"Gaming Consol G402\",\"ProductDecription\":\"Stylish design\",\"ProductBrand\":\"Panasonic\",\"PriceId\":401,\"Currency\":\"PLN\",\"Price\":563.17,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"},{\"PriceId\":402,\"Currency\":\"EUR\",\"Price\":131.11,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"},{\"PriceId\":403,\"Currency\":\"USD\",\"Price\":142.69,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"},{\"PriceId\":404,\"Currency\":\"DKK\",\"Price\":978.24,\"StartDate\":\"2024-01-18\",\"EndDate\":\"2025-01-16\"}";      
            //var test2 = JsonSerializer.Deserialize<Product>(tekst);


            return new($"ProductId: {outputParams.ProductId}", outputParams.StatusId, outputParams.Message);
        }

    }
}
