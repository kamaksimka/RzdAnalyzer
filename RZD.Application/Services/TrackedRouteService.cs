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
    }
}
