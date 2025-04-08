using Microsoft.EntityFrameworkCore;
using RZD.Application.Models;
using RZD.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Services
{
    public class TrainService
    {
        private readonly DataContext _context;
        private readonly TrackedRouteService _trackedRouteService;

        public TrainService(DataContext context, TrackedRouteService trackedRouteService)
        {
            _context = context;
            _trackedRouteService = trackedRouteService;
        }

        public async Task<List<TrainTableModel>> GetTrainsByTrackedRouteId(GetTrainsRequest request)
        {
            var startDate = request.Date.Date.ToUniversalTime();
            var endDate = startDate.AddDays(1).ToUniversalTime();

            var trainModels = await _context.Trains
                .Where(x => x.TrackedRouteId == request.TrackedRouteId &&  x.DepartureDateTime >= startDate && x.DepartureDateTime <= endDate)
                .Select(x => new TrainTableModel
                {
                    Id = x.Id,
                    ArrivalDateTime = x.ArrivalDateTime,
                    CarServices = x.CarServices,
                    CreatedDate = x.CreatedDate,
                    DepartureDateTime = x.DepartureDateTime,
                    TrainNumber = x.TrainNumber,
                    TripDuration = x.TripDuration,
                    MaxPrice = x.CarPlaces.Any()? x.CarPlaces.Select(x=> x.MaxPrice).Max():0,
                    MinPrice = x.CarPlaces.Any() ? x.CarPlaces.Select(x => x.MinPrice).Min() : 0,

                })
                .OrderBy(x => x.DepartureDateTime)
                .ToListAsync();

            return trainModels;
        }
    }
}
