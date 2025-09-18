using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiSDH.Common.Interfaces.Factories;

/// <summary>
///     Presentation layer interfaces have their implementations in the same layer.
/// </summary>
/// <remarks>
///     <see cref="ApiSDH.Common.Services.Factories.ResponseFactory" /> is used to convert
///     <see cref="Result{T}" /> objects from the application layer
///     into <see cref="IActionResult" /> objects returned by controllers
///     in the Presentation layer.
/// </remarks>
public interface IResponseFactory
{
    IActionResult CreateResponse<T>(Result<T> result);
}