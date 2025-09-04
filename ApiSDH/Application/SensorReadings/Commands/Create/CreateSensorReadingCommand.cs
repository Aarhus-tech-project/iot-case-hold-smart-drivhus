using Application.Common.Interfaces.Factories;
using Application.Common.Interfaces.Factories.Entity;
using Application.Common.Interfaces.Persistence;
using Application.Common.Models;
using Application.SensorReadings.Dtos;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Application.SensorReadings.Commands.Create;

public record CreateSensorReadingCommand(
    float Humidity,
    float Temperature,
    float Pressure,
    float DirtHumidity,
    float LightLevel) : IRequest<Result<SensorReadingDto>>;

public class CreateSensorReadingCommandHandler(
    ISensorContext sensorContext,
    IResultFactory resultFactory,
    ISensorReadingFactory sensorReadingFactory,
    IValidator<CreateSensorReadingCommand> validator,
    IMapper mapper)
    : IRequestHandler<CreateSensorReadingCommand, Result<SensorReadingDto>>
{
    public async Task<Result<SensorReadingDto>> Handle(CreateSensorReadingCommand request,
        CancellationToken cancellationToken)
    {
        var isValid = await validator.ValidateAsync(request, cancellationToken);

        if (!isValid.IsValid) return resultFactory.BadRequest<SensorReadingDto>();

        var sensorReading = sensorReadingFactory.Create(request);

        await sensorContext.SensorReadings.AddAsync(sensorReading, cancellationToken);
        await sensorContext.SaveChangesAsync(cancellationToken);

        return resultFactory.Created(mapper.Map<SensorReadingDto>(sensorReading));
    }

    public class CreateSensorReadingCommandValidator : AbstractValidator<CreateSensorReadingCommand>
    {
        public CreateSensorReadingCommandValidator()
        {
            RuleFor(c => c.Humidity).NotEmpty();
            RuleFor(c => c.Temperature).NotEmpty();
            RuleFor(c => c.Pressure).NotEmpty();
            RuleFor(c => c.DirtHumidity).NotEmpty();
            RuleFor(c => c.LightLevel).NotEmpty();
        }
    }
}