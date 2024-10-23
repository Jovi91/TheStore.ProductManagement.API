using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.API.Database;

public interface IDatabaseService
{
    Task<DbResults<Product[]?>> GetProductDataFromDb(string? productName, int? id);
    Task<DbResults<Product[]?>> GetAllProductDataFromDb(int? pageNumber, int? pageSize);
    Task<DbResults<string>> AddProductDataIntoDb(Product product);
    Task ApiErrorSaveToDb(ApiErrorLog errorLog);
}
