using ApiSDH.Common.Interfaces.Factories;
using Application.Configs.Commands.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiSDH.Controllers;

[ApiController]
[Route("api/config")]
public class ConfigController(IMediator mediator, IResponseFactory responseFactory)
{
    [HttpPatch]
    public async Task<IActionResult> UpdateConfig([FromBody] UpdateConfigCommand command)
    {
        var result = await mediator.Send(command);
        return responseFactory.CreateResponse(result);
    }
}