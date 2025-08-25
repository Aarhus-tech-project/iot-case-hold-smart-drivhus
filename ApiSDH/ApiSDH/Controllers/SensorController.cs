using ApiSDH.Common.Interfaces.Factories;
using Application.SensorReadings.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiSDH.Controllers;

[ApiController]
[Route("api/sensor")]
public class SensorController(IMediator mediator, IResponseFactory responseFactory) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateSensorData([FromBody] CreateSensorReadingCommand command)
    {
        var result = await mediator.Send(command);
        return responseFactory.CreateResponse(result);
    }
}