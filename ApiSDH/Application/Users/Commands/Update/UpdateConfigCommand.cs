using Application.Common.Interfaces.Factories;
using Application.Common.Interfaces.Persistence;
using Application.Common.Models;
using Application.Users.Dtos;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.Update;

public record UpdateConfigCommand(string phoneNumber, int waterLimit, int soilMoistureLimit, int co2Limit)
    : IRequest<Result<UserDto>>;

public class UpdateConfigCommandHandler(
    ISensorContext sensorContext,
    IResultFactory resultFactory,
    IMapper mapper,
    IValidator<UpdateConfigCommand> validator)
    : IRequestHandler<UpdateConfigCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(UpdateConfigCommand request, CancellationToken cancellationToken)
    {
        var isValid = await validator.ValidateAsync(request, cancellationToken);

        if (!isValid.IsValid) return resultFactory.BadRequest<UserDto>();

        var users = await sensorContext.Users.AsTracking().ToListAsync(cancellationToken);

        // Only 1 user allowed in the system.
        if (users.Count >= 2) return resultFactory.BadRequest<UserDto>();

        // Todo implement all properties 

        if (users[0].PhoneNumber != request.phoneNumber && request.phoneNumber != string.Empty)
            users[0].PhoneNumber = request.phoneNumber;

        if (users[0].WaterLimit != request.waterLimit && request.waterLimit is not 0)
            users[0].WaterLimit = request.waterLimit;

        if (users[0].SoilMoistureLimit != request.soilMoistureLimit && request.soilMoistureLimit is not 0)
            users[0].SoilMoistureLimit = request.soilMoistureLimit;

        if (users[0].Co2Limit != request.co2Limit && request.co2Limit is not 0)
            users[0].Co2Limit = request.co2Limit;

        await sensorContext.SaveChangesAsync(cancellationToken);

        return resultFactory.Ok(mapper.Map<UserDto>(users[0]));
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
        }
    }
}