using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TheStore.ProductManagement.API.Attributes;

namespace TheStore.ProductManagement.API.Models;

public class Product
{

    [JsonPropertyName("ProductName")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "Product Name must be at least 4 characters long.")]
    [Required]
    public string Name { get; set; }
    [JsonPropertyName("ProductDescription")]
    [Required]
    public string Description { get; set; }

    [JsonPropertyName("ProductBrand")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Brand must be at least 2 characters long.")]
    [Required]
    public string Brand { get; set; }
    [JsonPropertyName("Prices")]
    [MustContainAtLeastOneItem(ErrorMessage = "The Prices list must contain at least one item.")]
    [Required]
    public List<Price> Prices { get; set; }
}
