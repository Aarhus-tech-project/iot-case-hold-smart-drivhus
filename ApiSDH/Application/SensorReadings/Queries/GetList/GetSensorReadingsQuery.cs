using Application.Common.Interfaces.Factories;
using Application.Common.Interfaces.Persistence;
using Application.Common.Models;
using Application.SensorReadings.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.SensorReadings.Queries.GetList;

public record GetSensorReadingsQuery : IRequest<Result<List<SensorReadingDto>>>;

public class GetSensorReadingsQueryHandler(ISensorContext sensorContext, IResultFactory resultFactory, IMapper mapper)
    : IRequestHandler<GetSensorReadingsQuery, Result<List<SensorReadingDto>>>
{
    public async Task<Result<List<SensorReadingDto>>> Handle(GetSensorReadingsQuery request,
        CancellationToken cancellationToken)
    {
        // order by newest 
        var listToReturn = await sensorContext.SensorReadings.AsNoTracking().Take(200)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        if (listToReturn.Count is 0) return resultFactory.NotFound<List<SensorReadingDto>>();

        return resultFactory.Ok(mapper.Map<List<SensorReadingDto>>(listToReturn));
    }
}