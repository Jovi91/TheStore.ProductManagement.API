using System.Text.Json;

namespace TheStore.ProductManagement.API.Models
{
    public class Error
    {
        public int? StatusCode { get; set; }
        public List<string?> Message { get; set; }

        public Error() { }
        public Error(int? statusCode, List<string?> message)
        {
            StatusCode = statusCode;
            Message = message;
                
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
