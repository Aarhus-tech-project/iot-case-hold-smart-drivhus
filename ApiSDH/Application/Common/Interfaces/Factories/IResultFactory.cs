using Application.Common.Models;

namespace Application.Common.Interfaces.Factories;

public interface IResultFactory
{
    Result<T> Ok<T>(T value);
    Result<T> NotFound<T>();
    Result<T> NoContent<T>();
    Result<T> Created<T>(T value);
    Result<T> BadRequest<T>();
}