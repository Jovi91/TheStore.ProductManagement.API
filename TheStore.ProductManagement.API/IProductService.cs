using System.Text.Json;
using TheStore.ProductManagement.API.Model;
using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.API;

public interface IProductService
{
    Task<DbResults<Product[]>> GetProductDataFromDb(string? productName, int? id);
    Task<DbResults<string>> AddProductDataIntoDb(Product product);
}
