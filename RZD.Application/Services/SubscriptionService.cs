using Microsoft.EntityFrameworkCore;
using RZD.Application.Models;
using RZD.Database;
using RZD.Database.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Services
{
    public class SubscriptionService
    {
        private readonly DataContext _context;
        private readonly TrackedRouteService _trackedRouteService;
        private readonly TrainService _trainService;

        public SubscriptionService(DataContext context, TrackedRouteService trackedRouteService, TrainService trainService)
        {
            _context = context;
            _trackedRouteService = trackedRouteService;
            _trainService = trainService;
        }


        public async Task<List<SubscriptionModel>> GetSubscriptionsAsync(ClaimsPrincipal user)
        {
            var userId = await _context.Users
              .Where(x => x.Email == user.FindFirst(ClaimTypes.Email)!.Value)
              .Select(x => x.Id)
              .FirstOrDefaultAsync();

            var subs = await GetSubscriptionsAsync();

            return subs.Where(x => x.UserId ==  userId).ToList();
        }


        public async Task<List<SubscriptionModel>> GetSubscriptionsAsync()
        {
            var now = DateTimeOffset.UtcNow;
            var subscriptionModels = await _context.Subscriptions
                .Where(x => x.EndDepartureTime > now)
                .Select(x => new SubscriptionModel
                {
                    CarTypes = x.CarTypes,
                    EndDepartureTime = x.EndDepartureTime,
                    EndArrivalTime = x.EndArrivalTime,
                    IsLowerSeat = x.IsLowerSeat,
                    IsUpperSeat = x.IsUpperSeat,
                    IsAnySeat = x.IsAnySeat,
                    MaxPrice = x.MaxPrice,
                    MinPrice = x.MinPrice,
                    StartArrivalTime = x.StartArrivalTime,
                    StartDepartureTime = x.StartDepartureTime,
                    TrackedRouteId = x.TrackedRouteId,
                    UserEmail = x.User.Email,
                    UserId = x.User.Id,
                    TravelTimeInMinutes = x.TravelTimeInMinutes,
                })
                .ToListAsync();

            var trackedRoutes = await _trackedRouteService.TrackedRouteModelQuery.ToListAsync();

            foreach (var subscriptionModel in subscriptionModels)
            {
                var trackedRoute = trackedRoutes.First(x => x.Id == subscriptionModel.TrackedRouteId);

                subscriptionModel.OriginRegion = trackedRoute.OriginRegion;
                subscriptionModel.OriginStationName = trackedRoute.OriginName;
                subscriptionModel.DestinationRegion = trackedRoute.DestinationRegion;
                subscriptionModel.DestinationStationName = trackedRoute.DestinationName;
            }

            return subscriptionModels;
        }

        public async Task<List<TrainModel>> GetTrainsForSubsctription(SubscriptionModel subscriptionModel)
        {
            var route = _trackedRouteService.TrackedRouteModelQuery.First(x => x.Id == subscriptionModel.TrackedRouteId);

            var trains = await _trainService.PickUpTrain(new PickUpTrainRequest
            {
                CarTypes = subscriptionModel.CarTypes,
                EndArrivalTime = subscriptionModel.EndArrivalTime,
                EndDepartureTime = subscriptionModel.EndDepartureTime,
                IsLowerSeat = subscriptionModel.IsLowerSeat,
                IsAnySeat = subscriptionModel.IsAnySeat,
                IsUpperSeat = subscriptionModel.IsUpperSeat,
                MaxPrice = subscriptionModel.MaxPrice,
                MinPrice = subscriptionModel.MinPrice,
                StartArrivalTime = subscriptionModel.StartArrivalTime,
                StartDepartureTime = subscriptionModel.StartDepartureTime,
                TrackedRouteId = subscriptionModel.TrackedRouteId,
                TravelTimeInMinutes = subscriptionModel.TravelTimeInMinutes,
            })
                .Select(train => new TrainModel
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
                }).ToListAsync();

            return trains;
        }

        public async Task CreateSubscriptionAsync(PickUpTrainRequest request, ClaimsPrincipal user)
        {
            var userId = await _context.Users
              .Where(x => x.Email == user.FindFirst(ClaimTypes.Email)!.Value)
              .Select(x => x.Id)
              .FirstOrDefaultAsync();

            _context.Subscriptions.Add(new Subscription
            {
                UserId = userId,
                TravelTimeInMinutes = request.TravelTimeInMinutes,
                CarTypes = request.CarTypes,
                EndArrivalTime = request.EndArrivalTime,
                EndDepartureTime = request.EndDepartureTime,
                IsLowerSeat = request.IsLowerSeat,
                IsUpperSeat = request.IsUpperSeat,
                IsAnySeat = request.IsAnySeat,
                MaxPrice = request.MaxPrice,
                MinPrice = request.MinPrice,
                StartArrivalTime = request.StartArrivalTime,
                StartDepartureTime = request.StartDepartureTime,
                TrackedRouteId = request.TrackedRouteId,
                CarServices = request.CarServices,
                IsComplete = false,
                IsDelete = false,

            });

            await _context.SaveChangesAsync();
        }
    }
}
