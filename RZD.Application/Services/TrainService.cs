using Microsoft.EntityFrameworkCore;
using RZD.Application.Helpers;
using RZD.Application.Models;
using RZD.Common.Enums;
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
