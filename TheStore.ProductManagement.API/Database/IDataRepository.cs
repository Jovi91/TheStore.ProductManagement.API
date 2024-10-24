﻿using Insight.Database;
using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.API.Database;

public interface IDataRepository
{
    public record OutputParamsForGet(int StatusId, string Message, string ProductDetails);

    public record OutputParamsForPut(int StatusId, string Message, int ProductId);

    [Sql("ProductDetailsGet", Schema = "api")]
    Task<Product> GetProductData(string? ProductName, int? ProductId, OutputParamsForGet outParams);

    [Sql("ProductDetailsGetAll", Schema = "api")]
    Task<Product> GetAllProductData(int? StartRow, int? PageSize, OutputParamsForGet outParams);

    [Sql("ProductDetailsAdd", Schema = "api")]
    Task<Product> AddProductData(string? ProductJson, OutputParamsForPut outParams);

    [Sql("ProductApiErrorLog", Schema = "api")]
    Task ApiErrorsLog(string? ApiErrorJson);
}
