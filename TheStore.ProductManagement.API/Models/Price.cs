
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TheStore.ProductManagement.API.Attributes;

namespace TheStore.ProductManagement.API.Models;

public class Price
{
    
    [JsonPropertyName("Currency")]
    [Required]
    public string Currency { get; set; }

    [JsonPropertyName("Price")]
    [Required]
    public decimal PriceValue { get; set; }

    [JsonPropertyName("StartDate")]
    [Required]
    [DataType(DataType.Date)]
    public string StartDate  { get; set; }

    [JsonPropertyName("EndDate")]
    [DateRange]
    [Required]
    [DataType(DataType.Date)]
    public string EndDate { get; set; }
}
