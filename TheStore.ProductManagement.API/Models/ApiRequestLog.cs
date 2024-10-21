namespace TheStore.ProductManagement.API.Models
{
    public class ApiRequestLog
    {
        public string IpAddress { get; set; }
        public string Endpoint { get; set; }
        public DateTime RequestTimestamp { get; set; }
        public string RequestMethod { get; set; }
        public int RequestStatusCode { get; set; }
        public long ResponseTime { get; set; }
        public string RequestBody { get; set; }
        public string ErrorDetils { get; set; }
    }
}
