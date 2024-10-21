
using System.Text.Json.Serialization;
using TheStore.ProductManagement.API.Attributes;

namespace TheStore.ProductManagement.API.Models;

public class Price
{
    [JsonRequired]
    [JsonPropertyName("Currency")]
    public string Currency { get; set; }
    [JsonRequired]
    [JsonPropertyName("Price")]
    public decimal PriceValue { get; set; }

    [JsonRequired]
    [JsonPropertyName("StartDate")]
    public DateTime StartDate  { get; set; }

    [JsonRequired]
    [DateRange]
    [JsonPropertyName("EndDate")]
    public DateTime EndDate { get; set; }
}
