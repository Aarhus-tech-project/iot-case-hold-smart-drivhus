namespace Application.Common.Models;

public class Result<T>
{
    public int StatusCode { get; set; }
    public T? Value { get; set; }
}