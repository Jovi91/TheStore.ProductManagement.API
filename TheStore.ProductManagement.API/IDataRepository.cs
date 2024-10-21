using Insight.Database;
using System.Data.SqlTypes;
using System.Xml.Serialization;
using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.API
{
    public interface IDataRepository
    {
        public record OutputParamsForGet(int StatusId, string Message, string ProductDetails);

        public record OutputParamsForPut(int StatusId, string Message, int ProductId);
    
        [Sql("usp_ProductDetailsGet", Schema = "dbo")]
        Task<Product> GetProductData(string? ProductName, int? ProductId, OutputParamsForGet outParams);

        [Sql("usp_ProductDetailsAdd", Schema = "dbo")]
        Task<Product> AddProductData(string? ProductJson, OutputParamsForPut outParams);
    }
}
