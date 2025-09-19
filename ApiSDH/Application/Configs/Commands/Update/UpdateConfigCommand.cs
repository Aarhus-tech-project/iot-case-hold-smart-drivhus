using Application.Common.Interfaces.Factories;
using Application.Common.Interfaces.Persistence;
using Application.Common.Models;
using Application.Configs.Dtos;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Configs.Commands.Update;

/// <summary>
///     <see cref="UpdateConfigCommand" /> and <see cref="UpdateConfigCommandHandler" /> are responsible for
///     updating the user config.
///     The config includes the users phone number and sensor settings.
/// </summary>
public record UpdateConfigCommand(
    string phoneNumber,
    int waterLimit,
    int soilMoistureLimit,
    int co2Limit,
    int humiLimit,
    int lightLimit,
    int tempLimit)
    : IRequest<Result<ConfigDto>>;

public class UpdateConfigCommandHandler(
    ISensorContext sensorContext,
    IResultFactory resultFactory,
    IMapper mapper,
    IValidator<UpdateConfigCommand> validator)
    : IRequestHandler<UpdateConfigCommand, Result<ConfigDto>>
{
    public async Task<Result<ConfigDto>> Handle(UpdateConfigCommand request, CancellationToken cancellationToken)
    {
        var isValid = await validator.ValidateAsync(request, cancellationToken);

        if (!isValid.IsValid) return resultFactory.BadRequest<ConfigDto>();

        var configs = await sensorContext.Configs.AsTracking().ToListAsync(cancellationToken);

        // Only 1 config allowed in the system.
        if (configs.Count >= 2) return resultFactory.BadRequest<ConfigDto>();

        if (configs[0].PhoneNumber != request.phoneNumber && request.phoneNumber != string.Empty)
            configs[0].PhoneNumber = request.phoneNumber;

        if (configs[0].WaterLimit != request.waterLimit && request.waterLimit is not 0)
            configs[0].WaterLimit = request.waterLimit;

        if (configs[0].SoilMoistureLimit != request.soilMoistureLimit && request.soilMoistureLimit is not 0)
            configs[0].SoilMoistureLimit = request.soilMoistureLimit;

        if (configs[0].Co2Limit != request.co2Limit && request.co2Limit is not 0)
            configs[0].Co2Limit = request.co2Limit;

        if (configs[0].HumiLimit != request.humiLimit && request.humiLimit is not 0)
            configs[0].HumiLimit = request.humiLimit;

        if (configs[0].LightLimit != request.lightLimit && request.lightLimit is not 0)
            configs[0].LightLimit = request.lightLimit;

        if (configs[0].TempLimit != request.tempLimit && request.tempLimit is not 0)
            configs[0].TempLimit = request.tempLimit;

        await sensorContext.SaveChangesAsync(cancellationToken);

        return resultFactory.Ok(mapper.Map<ConfigDto>(configs[0]));
    }

    public class UpdateConfigCommandValidator : AbstractValidator<UpdateConfigCommand>
    {
        public UpdateConfigCommandValidator()
        {
            RuleFor(r => r.phoneNumber)
                .Matches(@"^\+45\d{8}$")
                .When(r => r.phoneNumber != string.Empty)
                .WithMessage("Invalid phone number");

            RuleFor(r => r.waterLimit)
                .GreaterThan(0)
                .LessThan(10000)
                .When(r => r.waterLimit is not 0)
                .WithMessage("Invalid water limit");

            RuleFor(r => r.soilMoistureLimit)
                .GreaterThan(0)
                .LessThan(10000)
                .When(r => r.soilMoistureLimit is not 0)
                .WithMessage("Invalid soil moisture limit");

            RuleFor(r => r.co2Limit)
                .GreaterThan(0)
                .LessThan(10000)
                .When(r => r.co2Limit is not 0)
                .WithMessage("Invalid co2 limit");

            RuleFor(r => r.humiLimit)
                .GreaterThan(0)
                .LessThan(10000)
                .When(r => r.humiLimit is not 0)
                .WithMessage("Invalid humi limit");

            RuleFor(r => r.lightLimit)
                .GreaterThan(0)
                .LessThan(10000)
                .When(r => r.lightLimit is not 0)
                .WithMessage("Invalid light limit");

            RuleFor(r => r.tempLimit)
                .GreaterThan(0)
                .LessThan(10000)
                .When(r => r.tempLimit is not 0)
                .WithMessage("Invalid temp limit");
        }
    }
}