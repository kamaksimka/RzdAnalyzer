using Microsoft.EntityFrameworkCore;
using RZD.Application.Models;
using RZD.Common.Exceptions;
using RZD.Database;
using RZD.Database.Models;

namespace RZD.Application.Services
{
    public class TrackedRouteService
    {
        private readonly DataContext _context;

        public TrackedRouteService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<TrackedRouteModel>> GetAllAsync()
        {
            var trackedRoutes = from tr in _context.TrackedRoutes
                                join co in _context.Cities on tr.OriginExpressCode equals co.ExpressCode into coJoined
                                from co in coJoined.DefaultIfEmpty()
                                join tso in _context.TrainStations on tr.OriginExpressCode equals tso.ExpressCode into tsoJoined
                                from tso in tsoJoined.DefaultIfEmpty()
                                join cd in _context.Cities on tr.DestinationExpressCode equals cd.ExpressCode into cdJoined
                                from cd in cdJoined.DefaultIfEmpty()
                                join tsd in _context.TrainStations on tr.OriginExpressCode equals tsd.ExpressCode into tsdJoined
                                from tsd in tsdJoined.DefaultIfEmpty()
                                where !tr.IsDeleted
                                select new TrackedRouteModel
                                {
                                    Id = tr.Id,
                                    OriginName = co != null ? co.Name : tso.Name,
                                    OriginRegion = co != null ? co.Region : tso.Region,
                                    DestinationName = cd != null ? cd.Name : tsd.Name,
                                    DestinationRegion = cd != null ? cd.Region : tsd.Region,
                                    CreatedDate = tr.CreatedDate
                                };

            return await trackedRoutes.ToListAsync();
        }

        public async Task DeleteAsync(DeleteTrackedRouteRequest request)
        {
            await _context.TrackedRoutes
                .Where(x => x.Id == request.TrackeRouteId)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.IsDeleted, true));
        }

        public async Task CreateAsync(CreateTrackedRouteRequest request)
        {
            if (await _context.TrackedRoutes.AnyAsync(x => x.OriginExpressCode == request.OriginExpressCode && x.DestinationExpressCode == request.DestinationExpressCode))
            {
                throw new BadRequestExeption("Такой маршрут уже добавлен в отслеживание");
            }

            await _context.TrackedRoutes.AddAsync(new TrackedRoute
            {
                OriginExpressCode = request.OriginExpressCode,
                DestinationExpressCode = request.DestinationExpressCode,
                CreatedDate = DateTimeOffset.UtcNow,
                IsDeleted = false,
            });

            await _context.SaveChangesAsync();
        }

        public async Task<List<TrainStationModel>> SuggestsAsync(SuggestsRequest request)
        {
            var cities = await _context.Cities.Where(x => x.Name.ToLower().Contains(request.Query.ToLower()))
                .Select(x => new TrainStationModel
                {
                    ExpressCode = x.ExpressCode,
                    Name = x.Name,
                    Region = x.Region,
                })
                .ToListAsync();

            var trainStation = await _context.TrainStations.Where(x => x.Name.ToLower().Contains(request.Query.ToLower()))
                .Select(x => new TrainStationModel
                {
                    ExpressCode = x.ExpressCode,
                    Name = x.Name,
                    Region = x.Region,
                })
                .ToListAsync();

            return cities.Union(trainStation).ToList().Distinct().ToList();
        }

        public async Task<RouteStatistic> GetRouteStatistic(RouteStatisticRequest request)
        {
            var routeStatistic = await (from tr in _context.TrackedRoutes
                                        join co in _context.Cities on tr.OriginExpressCode equals co.ExpressCode into coJoined
                                        from co in coJoined.DefaultIfEmpty()
                                        join tso in _context.TrainStations on tr.OriginExpressCode equals tso.ExpressCode into tsoJoined
                                        from tso in tsoJoined.DefaultIfEmpty()
                                        join cd in _context.Cities on tr.DestinationExpressCode equals cd.ExpressCode into cdJoined
                                        from cd in cdJoined.DefaultIfEmpty()
                                        join tsd in _context.TrainStations on tr.OriginExpressCode equals tsd.ExpressCode into tsdJoined
                                        from tsd in tsdJoined.DefaultIfEmpty()
                                        where !tr.IsDeleted && tr.Id == request.TrackedRouteId
                                        select new RouteStatistic
                                        {
                                            OriginStationName = co != null ? co.Name : tso.Name,
                                            OriginRegion = co != null ? co.Region : tso.Region,
                                            DestinationStationName = cd != null ? cd.Name : tsd.Name,
                                            DestinationRegion = cd != null ? cd.Region : tsd.Region,
                                            NumberTrains = tr.Trains.Count,
                                            NumberCarPlaces = tr.Trains.SelectMany(x => x.Cars).Count(),
                                            StartTrackedDate = tr.CreatedDate,
                                            MaxPrice = tr.Trains.Any() ? tr.Trains.SelectMany(x => x.Cars).Select(x => x.MaxPrice).Max() : null,
                                            MinPrice = tr.Trains.Any() ? tr.Trains.SelectMany(x => x.Cars).Select(x => x.MinPrice).Min() : null,
                                            FastestTrain = tr.Trains.Any() ? tr.Trains.Select(x => x.ArrivalDateTime - x.DepartureDateTime).Min() : null,
                                            SlowestTrain = tr.Trains.Any() ? tr.Trains.Select(x => x.ArrivalDateTime - x.DepartureDateTime).Max() : null,
                                        }).FirstOrDefaultAsync();

            return routeStatistic!;
        }
    }
}
