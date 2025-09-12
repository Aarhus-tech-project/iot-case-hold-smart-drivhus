using ApiSDH.Common.Interfaces.Factories;
using Application.Users.Commands.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiSDH.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IMediator mediator, IResponseFactory responseFactory)
{
    // [HttpPatch("{phoneNumber}")]
    // public async Task<IActionResult> UpdatePhoneNumber(string phoneNumber)
    // {
    //     var result = await mediator.Send(new UpdatePhoneNumberQuery(phoneNumber));
    //     return responseFactory.CreateResponse(result);
    // }

    [HttpPatch]
    public async Task<IActionResult> UpdateConfig([FromBody] UpdateConfigCommand command)
    {
        var result = await mediator.Send(command);
        return responseFactory.CreateResponse(result);
    }
}