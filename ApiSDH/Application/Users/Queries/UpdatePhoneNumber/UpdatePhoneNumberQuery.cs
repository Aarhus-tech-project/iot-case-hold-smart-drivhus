using Application.Common.Interfaces.Factories;
using Application.Common.Interfaces.Persistence;
using Application.Common.Models;
using Application.Users.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.UpdatePhoneNumber;

public record UpdatePhoneNumberQuery(string PhoneNumber) : IRequest<Result<UserDto>>;

public class UpdatePhoneNumberQueryHandler(ISensorContext sensorContext, IResultFactory resultFactory, IMapper mapper)
    : IRequestHandler<UpdatePhoneNumberQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(UpdatePhoneNumberQuery request, CancellationToken cancellationToken)
    {
        var user = await sensorContext.Users.AsTracking().FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            // server fejl, der skal værer 1 user. 
            throw new InvalidOperationException();

        user.PhoneNumber = request.PhoneNumber;

        await sensorContext.SaveChangesAsync(cancellationToken);
        return resultFactory.Ok(mapper.Map<UserDto>(user));
    }
}