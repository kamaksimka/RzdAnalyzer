using Microsoft.EntityFrameworkCore;
using RZD.Application.Helpers;
using RZD.Application.Models;
using RZD.Common.Enums;
using RZD.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        public async Task<TrainGridInitModel> GetTrainGridInitModel(TrainGridInitRequest request)
        {
            var trainGridInitModel = await _context.TrackedRoutes
                .Where(x => x.Id == request.TrackedRouteId)
                .Select(x => new TrainGridInitModel
                {
                    MinDate = x.Trains.Select(x => x.DepartureDateTime).Min().Date,
                    MaxDate = x.Trains.Select(x => x.DepartureDateTime).Max().Date,
                }).FirstAsync();

            return trainGridInitModel;
        }

        public async Task<List<TrainTableModel>> GetTrainsByTrackedRouteId(GetTrainsRequest request)
        {
            var startDate = request.DateFrom.Date.ToMoscowTime().ToUniversalTime();
            var endDate = request.DateTo.Date.ToMoscowTime().ToUniversalTime();

            var trainModels = await _context.Trains
                .Where(x => x.TrackedRouteId == request.TrackedRouteId && x.DepartureDateTime >= startDate && x.DepartureDateTime <= endDate)
                .Select(x => new TrainTableModel
                {
                    Id = x.Id,
                    ArrivalDateTime = x.ArrivalDateTime,
                    CarServices = x.CarServices,
                    CreatedDate = x.CreatedDate,
                    DepartureDateTime = x.DepartureDateTime,
                    TrainNumber = x.TrainNumber,
                    TripDuration = x.TripDuration,
                    MaxPrice = x.CarPlaces.Any() ? x.CarPlaces.Select(x => x.MaxPrice).Max() : 0,
                    MinPrice = x.CarPlaces.Any() ? x.CarPlaces.Select(x => x.MinPrice).Min() : 0,

                })
                .OrderBy(x => x.DepartureDateTime)
                .ToListAsync();

            return trainModels;
        }

        public async Task<Dictionary<DateTime, int>> GetFreePlacesPlot(TrainRequest request)
        {


            var historyCarPlaces = (await (from cp in _context.CarPlaces
                                           join eh in _context.EntityHistories on cp.Id equals eh.EntityId
                                           where cp.TrainId == request.TrainId && eh.EntityTypeId == (int)EntityTypes.CarPlace && eh.FieldName == nameof(cp.IsFree)
                                           select new
                                           {
                                               cp.Id,
                                               eh.OldFieldValue,
                                               eh.ChangedAt,
                                           })
                                   .OrderByDescending(x => x.ChangedAt)
                                   .ToListAsync())
                                   .Select(x => new
                                   {
                                       x.Id,
                                       IsFree = JsonSerializer.Deserialize<bool>(x.OldFieldValue),
                                       CurrentUntil = x.ChangedAt,
                                   })
                                   .ToList();

            var trainCarPlaces = _context.CarPlaces
                .Where(x => x.TrainId == request.TrainId)
                .Select(x => new
                {
                    x.Id,
                    x.IsFree,
                })
                .ToList();

            var freePlacesPlot = new Dictionary<DateTime, int>();




            var lastDate = DateTimeOffset.Now.FromMoscowTime().RoundHour();
            var lastFreePlacesCount = trainCarPlaces.Count(x => x.IsFree);

            freePlacesPlot.Add(lastDate, lastFreePlacesCount);

            foreach (var item in historyCarPlaces)
            {
                lastDate = item.CurrentUntil.FromMoscowTime().RoundHour();
                lastFreePlacesCount += item.IsFree ? 1 : -1;

                if (freePlacesPlot.ContainsKey(lastDate))
                {
                    freePlacesPlot[lastDate] = lastFreePlacesCount;
                }
                else
                {
                    freePlacesPlot.Add(lastDate, lastFreePlacesCount);
                }
            }

            return freePlacesPlot.OrderBy(x => x.Key).ToDictionary(k => k.Key,v => v.Value);
        }
    }
}
