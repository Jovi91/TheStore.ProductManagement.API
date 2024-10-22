namespace TheStore.ProductManagement.API.Models
{
    public class ApiErrorLog : Error
    {
        public string IpAddress { get; set; }
        public string Endpoint { get; set; }
        public DateTime RequestTimestamp { get; set; }
        public string RequestMethod { get; set; }

    }
}
