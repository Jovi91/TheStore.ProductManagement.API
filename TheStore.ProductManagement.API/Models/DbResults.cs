namespace TheStore.ProductManagement.API.Models;

public class DbResults<T>
{
    public T? Data { get; set; }
    public int Status { get; set; }
    public string Message { get; set; }

    public DbResults(T? data, int status, string message)
    {
        Data = data;
        Status = status;
        Message = message;
         
    }
}
