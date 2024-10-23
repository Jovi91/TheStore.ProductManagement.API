using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.API;

public interface IDatabaseService
{
    Task<DbResults<Product[]?>> GetProductDataFromDb(string? productName, int? id);
    Task<DbResults<string>> AddProductDataIntoDb(Product product);
    Task ApiErrorSaveToDb(ApiErrorLog errorLog);
}
