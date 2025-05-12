using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RZD.Application.Helpers;
using RZD.Application.Models;
using RZD.Common.Enums;
using RZD.Database;
using RZD.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
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

        public async Task<Dictionary<DateTime, decimal>> GetMinPricePlacesPlot(CarPlaceGroupingRequest request)
        {
            var dict = await GetCarPlaceGroupingByTime(request);

            return dict.Where(x => x.Value.Any(x => x.IsFree)).ToDictionary(k => k.Key, v => v.Value.Where(x => x.IsFree).Min(x => x.MinPrice));
        }

        public async Task<Dictionary<DateTime, decimal>> GetMaxPricePlacesPlot(CarPlaceGroupingRequest request)
        {
            var dict = await GetCarPlaceGroupingByTime(request);

            return dict.Where(x => x.Value.Any(x => x.IsFree)).ToDictionary(k => k.Key, v => v.Value.Where(x => x.IsFree).Max(x => x.MaxPrice));
        }

        public async Task<Dictionary<DateTime, int>> GetFreePlacesPlotByCarPlaceType(CarPlaceGroupingRequest request)
        {
            var dict = await GetCarPlaceGroupingByTime(request);

            return dict.ToDictionary(k => k.Key, v => v.Value.Count(x => x.IsFree));
        }

        public async Task<TrainModel> GetTrainAsync(TrainRequest request)
        {
            var train = await _context.Trains
                .FirstAsync(x => x.Id == request.TrainId);

            var route = _trackedRouteService.TrackedRouteModelQuery.First(x => x.Id == train.TrackedRouteId);

            var trainModel = new TrainModel
            {
                Id = train.Id,
                ArrivalDateTime = train.ArrivalDateTime,
                CreatedDate = train.CreatedDate,
                DepartureDateTime = train.DepartureDateTime,
                TrainBrandCode = train.TrainBrandCode,
                TrainDescription = train.TrainDescription,
                TrainNumber = train.TrainNumber,
                TripDistance = train.TripDistance,
                TripDuration = train.TripDuration,
                CarServices = train.CarServices,
                ArrivalStation = route.DestinationName,
                DepartureStation = route.OriginName
            };

            return trainModel;
        }

        public async Task<List<TrainingFreePlacesModel>> GetTrainingFreePlacesModel(long trackedRouteId)
        {
            var models = new List<TrainingFreePlacesModel>();

            var now = DateTime.UtcNow;

            var trains = _context.Trains
                .Where(x => x.TrackedRouteId == trackedRouteId && x.DepartureDateTime >= new DateTime(2025,4,28))
                .OrderBy(x => x.Id)
                .ToList();

            var carTypes = new List<string> { "Compartment", "ReservedSeat",
                                            "Luxury",
                                            "Sedentary","Soft"};

            foreach (var train in trains)
            {
                foreach (var carType in carTypes)
                {
                    var cp = await GetCarPlaceGroupingByTime(new CarPlaceGroupingRequest { CarType = carType, TrainId = train.Id });
                    if (cp.OrderBy(x => x.Key).First().Value.Count == 0)
                    {
                        continue;
                    }
                    models.Add(new TrainingFreePlacesModel
                    {
                        TrainId = train.Id,
                        CarType = carType,
                        ArrivalDateTime = train.ArrivalDateTime.FromMoscowTime(),
                        DepartureDateTime = train.DepartureDateTime.FromMoscowTime(),
                        CountPlace = cp.First().Value.Count,
                        StartMinPrice = cp.OrderBy(x => x.Key).First().Value.Min(x => x.MinPrice),
                        StartMaxPrice = cp.OrderBy(x => x.Key).First().Value.Max(x => x.MaxPrice),
                        FreePlaces = cp.Where(x => x.Key > new DateTime(2025, 4, 27)).ToDictionary(k => k.Key, v => v.Value.Count(x => x.IsFree)),
                        MaxPrice = cp.Where(x => x.Key > new DateTime(2025, 4, 27)).Where(x => x.Value.Any(x => x.IsFree)).ToDictionary(k => k.Key, v => v.Value.Where(x => x.IsFree).Max(x => x.MaxPrice)),
                        MinPrice = cp.Where(x => x.Key > new DateTime(2025, 4, 27)).Where(x => x.Value.Any(x => x.IsFree)).ToDictionary(k => k.Key, v => v.Value.Where(x => x.IsFree).Min(x => x.MinPrice))
                    });
                }
            }


            return models;
        }

        public async Task<List<TrainTableModel>> PickUpTrainAsync(PickUpTrainRequest request)
        {
            var trains = await PickUpTrain(request)
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

            return trains;
        }

        public IQueryable<Train> PickUpTrain(PickUpTrainRequest request)
        {
            var endArrivalTime = request.EndArrivalTime.ToUniversalTime();
            var startArrivalTime = request.StartArrivalTime.ToUniversalTime();

            var endDepartureTime = request.EndDepartureTime?.ToUniversalTime();
            var startDepartureTime = request.StartDepartureTime?.ToUniversalTime();


            var trains = _context.Trains.Where(x => x.TrackedRouteId == request.TrackedRouteId
                && (startArrivalTime == null || x.ArrivalDateTime >= startArrivalTime && x.ArrivalDateTime <= endArrivalTime)
                && x.DepartureDateTime >= startDepartureTime && x.DepartureDateTime <= endDepartureTime
                && x.TripDuration <= request.TravelTimeInMinutes
                && x.CarPlaces.Any(x => x.IsFree && (request.MinPrice == null || x.MinPrice >= request.MinPrice)
                        && (request.MaxPrice == null || x.MaxPrice <= request.MaxPrice)
                        && request.CarTypes.Contains(x.CarType)
                        && (!request.IsUpperSeat || x.CarPlaceType.Contains("Upper"))
                        && (!request.IsLowerSeat || x.CarPlaceType.Contains("Lower"))));

             return trains;
        }

        private async Task<Dictionary<DateTime, List<CarPlaceShortModel>>> GetCarPlaceGroupingByTime(CarPlaceGroupingRequest request)
        {
            var carPlacesByTime = new Dictionary<DateTime, List<CarPlaceShortModel>>();


            var train = _context.Trains.First(x => x.Id == request.TrainId);

            var historyCarPlaces = (await (from cp in _context.CarPlaces
                                           join eh in _context.EntityHistories on cp.Id equals eh.EntityId
                                           where cp.TrainId == request.TrainId
                                               && cp.CarType == request.CarType
                                               && eh.EntityTypeId == (int)EntityTypes.CarPlace
                                           select new
                                           {
                                               EntityId = cp.Id,
                                               eh.FieldName,
                                               eh.OldFieldValue,
                                               eh.ChangedAt,
                                           })
                .OrderByDescending(x => x.ChangedAt)
                .ToListAsync());

            var trainCarPlaces = _context.CarPlaces
                .Where(x => x.TrainId == request.TrainId && x.CarType == request.CarType)
                .ToList();

            carPlacesByTime.Add(DateTimeOffset.Now.FromMoscowTime(), trainCarPlaces.Select(x => new CarPlaceShortModel
            {
                Id = x.Id,
                CarType = x.CarType,
                IsFree = x.IsFree,
                MaxPrice = x.MaxPrice,
                MinPrice = x.MinPrice
            }).ToList());


            var historyCarPlacesGrouping = historyCarPlaces.GroupBy(x => Hour(x.ChangedAt));

            foreach (var historyByTime in historyCarPlacesGrouping)
            {
                var carPlaceShortModels = carPlacesByTime.Last().Value.Select(x => new CarPlaceShortModel
                {
                    Id = x.Id,
                    CarType = x.CarType,
                    IsFree = x.IsFree,
                    MaxPrice = x.MaxPrice,
                    MinPrice = x.MinPrice
                }).ToList();

                foreach (var changed in historyByTime)
                {
                    var carPlaceShortModel = carPlaceShortModels.First(x => x.Id == changed.EntityId);

                    switch (changed.FieldName)
                    {
                        case nameof(CarPlace.MinPrice):
                            {
                                carPlaceShortModel.MinPrice = JsonSerializer.Deserialize<decimal>(changed.OldFieldValue);
                                break;
                            }
                        case nameof(CarPlace.MaxPrice):
                            {
                                carPlaceShortModel.MaxPrice = JsonSerializer.Deserialize<decimal>(changed.OldFieldValue);
                                break;
                            }
                        case nameof(CarPlace.IsFree):
                            {
                                carPlaceShortModel.IsFree = JsonSerializer.Deserialize<bool>(changed.OldFieldValue);
                                break;
                            }
                    }
                }

                carPlacesByTime.Add(historyByTime.Key, carPlaceShortModels);
            }

            var carPlacesByTimeList = carPlacesByTime.ToList();
            var carPlacesByTimeWithRightTime = new Dictionary<DateTime, List<CarPlaceShortModel>>();

            for (var i = 0; i < carPlacesByTimeList.Count; i++)
            {
                if (i == carPlacesByTimeList.Count - 1)
                {
                    carPlacesByTimeWithRightTime.Add(train.CreatedDate.FromMoscowTime(), carPlacesByTimeList[i].Value);
                }
                else
                {
                    carPlacesByTimeWithRightTime.Add(carPlacesByTimeList[i + 1].Key, carPlacesByTimeList[i].Value);
                }
            }

            return carPlacesByTimeWithRightTime.OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value);
        }

        private async Task<Dictionary<DateTime, List<CarPlaceShortModel>>> GetCarPlaceGroupingByTime(long trainId)
        {
            var carPlacesByTime = new Dictionary<DateTime, List<CarPlaceShortModel>>();


            var train = _context.Trains.First(x => x.Id == trainId);

            var historyCarPlaces = (await (from cp in _context.CarPlaces
                                           join eh in _context.EntityHistories on cp.Id equals eh.EntityId
                                           where cp.TrainId == trainId
                                               && eh.EntityTypeId == (int)EntityTypes.CarPlace
                                           select new
                                           {
                                               EntityId = cp.Id,
                                               eh.FieldName,
                                               eh.OldFieldValue,
                                               eh.ChangedAt,
                                           })
                .OrderByDescending(x => x.ChangedAt)
                .ToListAsync());

            var trainCarPlaces = _context.CarPlaces
                .Where(x => x.TrainId == trainId)
                .ToList();

            carPlacesByTime.Add(DateTimeOffset.Now.FromMoscowTime(), trainCarPlaces.Select(x => new CarPlaceShortModel
            {
                Id = x.Id,
                CarType = x.CarType,
                IsFree = x.IsFree,
                MaxPrice = x.MaxPrice,
                MinPrice = x.MinPrice
            }).ToList());


            var historyCarPlacesGrouping = historyCarPlaces.GroupBy(x => Hour(x.ChangedAt));

            foreach (var historyByTime in historyCarPlacesGrouping)
            {
                var carPlaceShortModels = carPlacesByTime.Last().Value.Select(x => new CarPlaceShortModel
                {
                    Id = x.Id,
                    CarType = x.CarType,
                    IsFree = x.IsFree,
                    MaxPrice = x.MaxPrice,
                    MinPrice = x.MinPrice
                }).ToList();

                foreach (var changed in historyByTime)
                {
                    var carPlaceShortModel = carPlaceShortModels.First(x => x.Id == changed.EntityId);

                    switch (changed.FieldName)
                    {
                        case nameof(CarPlace.MinPrice):
                            {
                                carPlaceShortModel.MinPrice = JsonSerializer.Deserialize<decimal>(changed.OldFieldValue);
                                break;
                            }
                        case nameof(CarPlace.MaxPrice):
                            {
                                carPlaceShortModel.MaxPrice = JsonSerializer.Deserialize<decimal>(changed.OldFieldValue);
                                break;
                            }
                        case nameof(CarPlace.IsFree):
                            {
                                carPlaceShortModel.IsFree = JsonSerializer.Deserialize<bool>(changed.OldFieldValue);
                                break;
                            }
                    }
                }

                carPlacesByTime.Add(historyByTime.Key, carPlaceShortModels);
            }

            var carPlacesByTimeList = carPlacesByTime.ToList();
            var carPlacesByTimeWithRightTime = new Dictionary<DateTime, List<CarPlaceShortModel>>();

            for (var i = 0; i < carPlacesByTimeList.Count; i++)
            {
                if (i == carPlacesByTimeList.Count - 1)
                {
                    carPlacesByTimeWithRightTime.Add(train.CreatedDate.FromMoscowTime(), carPlacesByTimeList[i].Value);
                }
                else
                {
                    carPlacesByTimeWithRightTime.Add(carPlacesByTimeList[i + 1].Key, carPlacesByTimeList[i].Value);
                }
            }

            return carPlacesByTimeWithRightTime.OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value);
        }

        private DateTime Hour(DateTimeOffset? date = null) => date == null
            ? DateTimeOffset.Now.FromMoscowTime().RoundHour()
            : date.Value.FromMoscowTime().RoundHour();
    }
}
