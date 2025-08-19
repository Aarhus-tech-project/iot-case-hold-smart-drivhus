using Application.Common.Interfaces.Factories;
using Application.Common.Interfaces.Persistence;
using Application.Common.Models;
using Application.SensorReadings.Commands.Dtos;
using MediatR;

namespace Application.SensorReadings.Commands.Create;

public record CreateSensorReadingCommand(string Name) : IRequest<Result<SensorReadingDto>>;

public class CreateSensorReadingCommandHandler(ISensorContext sensorContext, IResultFactory resultFactory)
    : IRequestHandler<CreateSensorReadingCommand, Result<SensorReadingDto>>
{
    public async Task<Result<SensorReadingDto>> Handle(CreateSensorReadingCommand request,
        CancellationToken cancellationToken)
    {
        return resultFactory.Created(new SensorReadingDto());
    }
}