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
    int lux,
    int soil,
    int temp,
    int pressure,
    int humidity,
    int co2,
    int water
) : IRequest<Result<SensorReadingDto>>;

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
            RuleFor(c => c.lux).NotNull();
            RuleFor(c => c.soil).NotNull();
            RuleFor(c => c.temp).NotNull();
            RuleFor(c => c.pressure).NotNull();
            RuleFor(c => c.humidity).NotNull();
            RuleFor(c => c.co2).NotNull();
            RuleFor(c => c.water).NotNull();
        }
    }
}