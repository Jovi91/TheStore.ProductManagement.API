using System.Text.Json;

namespace TheStore.ProductManagement.API.Models
{
    public class Error
    {
        public int? StatusCode { get; set; }
        public List<string?> Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
