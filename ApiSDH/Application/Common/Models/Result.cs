namespace Application.Common.Models;

/// <summary>
///     <see cref="Result{T}" /> is our custom return type for command/query handlers.
/// </summary>
public class Result<T>
{
    public int StatusCode { get; set; }
    public T? Value { get; set; }
}