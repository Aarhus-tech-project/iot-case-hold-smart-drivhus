using Application.Common.Models;

namespace Application.Common.Interfaces.Factories;

/// <summary>
///     Application Interfaces have their implementations in the infrastructure layer.
///     <see cref="IResultFactory" /> is used in command/query handlers to create the response.
///     <see cref="IResultFactory" />
/// </summary>
public interface IResultFactory
{
    Result<T> Ok<T>(T value);
    Result<T> NotFound<T>();
    Result<T> NoContent<T>();
    Result<T> Created<T>(T value);
    Result<T> BadRequest<T>();
}