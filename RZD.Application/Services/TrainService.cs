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

            return freePlacesPlot.OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value);
        }

        public async Task<List<CarPlaceTypeModel>> GetCarPlaceTypes(TrainRequest request)
        {
            var carPlaceTypes = await _context.CarPlaces
                .Where(x => x.TrainId == request.TrainId)
                .Select(x => new CarPlaceTypeModel
                {
                    CarPlaceType = x.CarPlaceType,
                    CarSubType = x.CarSubType,
                    CarType = x.CarType,
                    ServiceClass = x.ServiceClass,
                }).Distinct()
                .ToListAsync();

            return carPlaceTypes;
        }


        public async Task<Dictionary<DateTime, decimal>> GetMinPricePlacesPlot(GetPricePlacesPlotRequest request)
        {
            var historyCarPlaces = (await (from cp in _context.CarPlaces
                                           join eh in _context.EntityHistories on cp.Id equals eh.EntityId
                                           where cp.TrainId == request.TrainId && cp.CarPlaceType == request.CarPlaceType.CarPlaceType
                                               && cp.CarSubType == request.CarPlaceType.CarSubType && cp.ServiceClass == request.CarPlaceType.ServiceClass
                                               && cp.CarType == request.CarPlaceType.CarType
                                               && eh.EntityTypeId == (int)EntityTypes.CarPlace && (eh.FieldName == nameof(cp.MinPrice))
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
                                      MinPrice = JsonSerializer.Deserialize<decimal>(x.OldFieldValue),
                                      CurrentUntil = x.ChangedAt,
                                  })
                                  .ToList();

            var trainCarPlaces = _context.CarPlaces
                .Where(x => x.TrainId == request.TrainId && x.IsFree
                    && x.CarSubType == request.CarPlaceType.CarSubType && x.ServiceClass == request.CarPlaceType.ServiceClass
                    && x.CarPlaceType == request.CarPlaceType.CarPlaceType && x.CarType == request.CarPlaceType.CarType)
                .Select(x => new
                {
                    x.Id,
                    x.MinPrice,
                })
                .ToList();

            var minPricePlacesPlot = new Dictionary<DateTime, decimal>();

            var now = DateTimeOffset.Now.FromMoscowTime().RoundHour();

            minPricePlacesPlot.Add(now, trainCarPlaces.Select(x => x.MinPrice).Min());

            foreach (var item in historyCarPlaces)
            {
                var date = item.CurrentUntil.FromMoscowTime();

                if (minPricePlacesPlot.ContainsKey(date))
                {
                    minPricePlacesPlot[date] = item.MinPrice;
                }
                else
                {
                    minPricePlacesPlot.Add(date, item.MinPrice);
                }
            }


            return minPricePlacesPlot;

        }

        public async Task<Dictionary<DateTime, int>> GetFreePlacesPlotByCarPlaceType(GetPricePlacesPlotRequest request)
        {


            var historyCarPlaces = (await (from cp in _context.CarPlaces
                                           join eh in _context.EntityHistories on cp.Id equals eh.EntityId
                                           where cp.TrainId == request.TrainId 
                                               && (request.CarPlaceType.CarPlaceType == null || cp.CarPlaceType == request.CarPlaceType.CarPlaceType )
                                               && (request.CarPlaceType.CarType == null || cp.CarType == request.CarPlaceType.CarType)
                                               && (request.CarPlaceType.CarSubType == null || cp.CarSubType == request.CarPlaceType.CarSubType)
                                               && (request.CarPlaceType.ServiceClass == null || cp.ServiceClass == request.CarPlaceType.ServiceClass)
                                           && eh.EntityTypeId == (int)EntityTypes.CarPlace && eh.FieldName == nameof(cp.IsFree)
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
                .Where(x => x.TrainId == request.TrainId
                 && (request.CarPlaceType.CarPlaceType == null || x.CarPlaceType == request.CarPlaceType.CarPlaceType)
                                               && (request.CarPlaceType.CarType == null || x.CarType == request.CarPlaceType.CarType)
                                               && (request.CarPlaceType.CarSubType == null || x.CarSubType == request.CarPlaceType.CarSubType)
                                               && (request.CarPlaceType.ServiceClass == null || x.ServiceClass == request.CarPlaceType.ServiceClass))
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

            return freePlacesPlot.OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value);
        }

        public async Task<TrainModel> GetTrainAsync(TrainRequest request)
        {
            var train = await _context.Trains
                .Where(x => x.Id == request.TrainId)
                .Select(x => new TrainModel
                {
                    Id = x.Id,
                    ArrivalDateTime = x.ArrivalDateTime,
                    CreatedDate = x.CreatedDate,
                    DepartureDateTime = x.DepartureDateTime,
                    TrainBrandCode = x.TrainBrandCode,
                    TrainDescription = x.TrainDescription,
                    TrainNumber = x.TrainNumber,
                    TripDistance = x.TripDistance,
                    TripDuration = x.TripDuration,
                    CarServices = x.CarServices,

                })
                .FirstOrDefaultAsync();

            var carPlaces = await _context.CarPlaces.Where(x => x.TrainId == request.TrainId)
                .Select(x => new CarPlaceModel
                {
                    Id = x.Id,
                    CarNumber = x.CarNumber,
                    CarPlaceNumber = x.CarPlaceNumber,
                    CarPlaceType = x.CarPlaceType,
                    CarSubType = x.CarSubType,
                    CarType = x.CarType,
                    IsFree = x.IsFree,
                    CreatedDate = x.CreatedDate,
                    MinPrice = x.MinPrice,
                    MaxPrice = x.MaxPrice,
                    PassengerSpecifyingRules = x.PassengerSpecifyingRules,
                    PlaceReservationType = x.PlaceReservationType,
                    ServiceClass = x.ServiceClass,
                    ServiceCost = x.ServiceCost,
                    Services = x.Services, 
                    TripDirection = x.TripDirection,
                })
                .ToListAsync();
            train.CarPlaces = carPlaces;


            var trainHistory = await _context.EntityHistories
                .Where(x => x.EntityId == request.TrainId && x.EntityTypeId == (int)EntityTypes.Train)
                .Select(x => new EntityHistoryModel
                {
                    ChangedAt = x.ChangedAt,
                    FieldName = x.FieldName,
                    OldFieldValue = x.OldFieldValue
                })
                .GroupBy(x => x.FieldName)
                .ToDictionaryAsync(k => k.Key,v => v.ToList());
            train.HistoryOfChanges = trainHistory;

            var carPlacesIds = carPlaces.Select(x => x.Id).ToList();
            var carPlacesHistory = await _context.EntityHistories
                .Where(x => carPlacesIds.Contains(x.EntityId) && x.EntityTypeId == (int)EntityTypes.CarPlace)
                .GroupBy(x => x.EntityId)
                .ToDictionaryAsync(k => k.Key, v => v.Select(x => new EntityHistoryModel
                {
                    ChangedAt = x.ChangedAt,
                    FieldName = x.FieldName,
                    OldFieldValue = x.OldFieldValue
                }).ToList());
            
            foreach (var carPlace in train.CarPlaces)
            {
                carPlace.HistoryOfChanges = carPlacesHistory
                    .Where(x => x.Key == carPlace.Id)
                    .SelectMany(x => x.Value)
                    .GroupBy(x => x.FieldName)
                    .ToDictionary(x => x.Key, v => v.ToList());
            }
               

            return train;
        }
    }
}
