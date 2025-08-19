using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiSDH.Common.Interfaces.Factories;

public interface IResponseFactory
{ 
    IActionResult CreateResponse<T>(Result<T> result);
}