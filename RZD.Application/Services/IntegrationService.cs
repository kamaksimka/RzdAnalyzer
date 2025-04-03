using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using RZD.API;
using RZD.Application.Helpers;
using RZD.Common.Configs;
using RZD.Common.Enums;
using RZD.Database;
using RZD.Database.Models;
using RZD.API.Models.CarPricing;
using RZD.API.Models.TrainPricing;
using System.Diagnostics;
using System.Text.Json;

namespace RZD.Application.Services
{
    public class IntegrationService : IDisposable
    {
        private readonly DataContext _ctx;
        private readonly ILogger<IntegrationService> logger;
        private readonly RzdApi _api;
        RzdConfig rzdConfig;

        public IntegrationService(DataContext ctx, IOptions<RzdConfig> options, ILogger<IntegrationService> logger)
        {
            _ctx = ctx;
            rzdConfig = options.Value;
            this.logger = logger;
            _api = new RzdApi(rzdConfig);
        }

        public void Dispose()
        {
            _api.Dispose();
        }

        public async Task RefreshCitiesAsync()
        {
            logger.LogInformation("Started RefreshCitiesAsync");

            foreach (var ch in AlphabetHelper.RussianAlphabet)
            {
                var suggests = await _api.SuggestsAsync(ch);

                #region create-update Cities

                if (suggests.City != null)
                {
                    var cities = suggests.City;

                    var dbCities = await _ctx.Cities
                        .Where(x => cities.Select(y => y.NodeId).ToList().Contains(x.NodeId))
                        .ToListAsync();

                    var newDbCities = cities
                        .Where(x => !dbCities.Any(y => y.NodeId == x.NodeId))
                        .Select(x => new City
                        {
                            NodeId = x.NodeId,
                            ExpressCode = x.ExpressCode,
                            ExpressCodes = x.ExpressCodes,
                            ForeignCode = x.ForeignCode,
                            Name = x.Name,
                            Region = x.Region,
                            CreatedDate = DateTimeOffset.Now.ToUniversalTime(),
                        }).ToList();

                    await _ctx.Cities.AddRangeAsync(newDbCities);

                    await _ctx.SaveChangesAsync();
                }

                #endregion

                #region create-update Trains

                if (suggests.Train != null)
                {

                    var trainStations = suggests.Train;

                    var dbTrainStations = await _ctx.TrainStations
                        .Where(x => trainStations.Select(y => y.NodeId).ToList().Contains(x.NodeId))
                        .ToListAsync();

                    var newDbTrainStations = trainStations
                        .Where(x => !dbTrainStations.Any(y => y.NodeId == x.NodeId))
                        .Select(x => new TrainStation
                        {
                            NodeId = x.NodeId,
                            ExpressCode = x.ExpressCode,
                            ForeignCode = x.ForeignCode,
                            Name = x.Name,
                            Region = x.Region,
                            CreatedDate = DateTimeOffset.Now.ToUniversalTime(),
                        }).ToList();

                    await _ctx.TrainStations.AddRangeAsync(newDbTrainStations);

                    await _ctx.SaveChangesAsync();
                }
                #endregion
            }

            logger.LogInformation("Finished RefreshCitiesAsync");
        }

        public async Task RefreshTrainsAsync()
        {
            logger.LogInformation($"Started RefreshTrainsAsync");
            var dtStart = DateTime.Now;

            var trackedRoutes = _ctx.TrackedRoutes.ToList();

            foreach (var trackedRoute in trackedRoutes)
            {
                var dateStart = DateTime.Now.Date;
                var dateEnd = dateStart.AddDays(rzdConfig.NumberOfDay);
                for (var date = dateStart; date < dateEnd; date = date.AddDays(1))
                {
                    try
                    {
                        var trainResponse = await _api.TrainPricingAsync(trackedRoute.OriginExpressCode, trackedRoute.DestinationExpressCode, date);
                        logger.LogInformation($"Found {trainResponse.Trains.Count} trains for OriginExpressCode:{trackedRoute.OriginExpressCode}, DestinationExpressCode: {trackedRoute.DestinationExpressCode}, date:{date}");

                        foreach (var train in trainResponse.Trains)
                        {
                            var dbTrain = await _ctx.Trains
                                .Where(x => x.TrainNumber == train.TrainNumber
                                    && x.OriginStationCode == train.OriginStationCode
                                    && x.DestinationStationCode == train.DestinationStationCode
                                    && x.DepartureDateTime == train.DepartureDateTime.ToUniversalTime())
                                .FirstOrDefaultAsync();

                            if (dbTrain == null)
                            {
                                dbTrain = new Train()
                                {
                                    CreatedDate = DateTimeOffset.UtcNow
                                };
                                await UpdateDbTrainAsync(dbTrain, train);
                                await _ctx.Trains.AddAsync(dbTrain);
                                await _ctx.SaveChangesAsync();
                            }
                            else
                            {
                                var changes = GetChangesFromTrain(dbTrain, train);
                                if (changes.Any())
                                {
                                    await UpdateDbTrainAsync(dbTrain, train);

                                    foreach (var change in changes)
                                    {
                                        var entityHistory = new EntityHistory
                                        {
                                            ChangedAt = DateTimeOffset.Now.ToUniversalTime(),
                                            FieldName = change.Key,
                                            OldFieldValue = change.Value,
                                            EntityId = dbTrain.Id,
                                            EntityTypeId = (int)EntityTypes.Train,
                                        };

                                        await _ctx.EntityHistories.AddAsync(entityHistory);
                                    }

                                    await _ctx.SaveChangesAsync();
                                }
                            }

                            await RefreshCarsAsync(train.OriginStationCode, train.DestinationStationCode, train.DepartureDateTime.DateTime, train.TrainNumber, dbTrain.Id);
                        }
                    }
                    catch(Exception ex)
                    {
                        logger.LogError(ex, "Error in RefreshTrainsAsync");
                    }
                }
            }
            var dtFinish = DateTime.Now;
            var timeTaken = dtFinish - dtStart;
            logger.LogInformation($"Finished RefreshTrainsAsync. Time taken: {timeTaken}");
        }

        public async Task RefreshCarsAsync(string origin, string destination, DateTime departureDate, string trainNumber, long dbTrainId)
        {
            logger.LogInformation($"RefreshCarsAsync for origin:{origin}, destination:{destination}, departureDate:{departureDate}, trainNumber:{trainNumber}");
            try
            {


                var carsResponse = await _api.CarPricingAsync(origin, destination, departureDate, trainNumber);

                foreach (var car in carsResponse.Cars)
                {
                    var dbCar = await _ctx.Cars
                                .Where(x => x.TrainId == dbTrainId
                                    && x.CarNumber == car.CarNumber
                                    && x.CarPlaceType == car.CarPlaceType
                                    && x.CarType == car.CarType
                                    && x.CarSubType == car.CarSubType)
                                .FirstOrDefaultAsync();

                    if (dbCar == null)
                    {
                        dbCar = new Car()
                        {
                            TrainId = dbTrainId,
                            CreatedDate = DateTimeOffset.UtcNow
                        };

                        await UpdateDbCarAsync(dbCar, car);
                        await _ctx.Cars.AddAsync(dbCar);
                        await _ctx.SaveChangesAsync();
                    }
                    else
                    {
                        var changes = GetChangesFromCar(dbCar, car);
                        if (changes.Any())
                        {
                            await UpdateDbCarAsync(dbCar, car);

                            foreach (var change in changes)
                            {
                                var entityHistory = new EntityHistory
                                {
                                    ChangedAt = DateTimeOffset.Now.ToUniversalTime(),
                                    FieldName = change.Key,
                                    OldFieldValue = change.Value,
                                    EntityId = dbCar.Id,
                                    EntityTypeId = (int)EntityTypes.Car,
                                };

                                await _ctx.EntityHistories.AddAsync(entityHistory);
                            }

                            await _ctx.SaveChangesAsync();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error in RefreshCarsAsync");
            }
        }

        private async Task UpdateDbCarAsync(Car newDbCar, RzdCar car)
        {
            newDbCar.ArePlacesForBusinessTravelBooking = car.ArePlacesForBusinessTravelBooking;
            newDbCar.ArrivalDateTime = car.ArrivalDateTime.ToUniversalTime();
            newDbCar.AvailabilityIndication = car.AvailabilityIndication;
            newDbCar.CarNumber = car.CarNumber;
            newDbCar.CarPlaceType = car.CarPlaceType;
            newDbCar.CarSubType = car.CarSubType;
            newDbCar.CarType = car.CarType;
            newDbCar.DestinationStationCode = car.DestinationStationCode;
            newDbCar.FreePlaces = car.FreePlaces;
            newDbCar.HasDynamicPricing = car.HasDynamicPricing;
            newDbCar.HasFssBenefit = car.HasFssBenefit;
            newDbCar.HasGenderCabins = car.HasGenderCabins;
            newDbCar.HasNonRefundableTariff = car.HasNonRefundableTariff;
            newDbCar.HasPlacesNearBabies = car.HasPlacesNearBabies;
            newDbCar.HasPlacesNearPets = car.HasPlacesNearPets;
            newDbCar.IsAdditionalMealOptionPossible = car.IsAdditionalMealOptionPossible;
            newDbCar.IsAdditionalPassengerAllowed = car.IsAdditionalPassengerAllowed;
            newDbCar.IsBranded = car.IsBranded;
            newDbCar.IsBuffet = car.IsBuffet;
            newDbCar.IsCarTransportationCoach = car.IsCarTransportationCoach;
            newDbCar.IsChildTariffTypeAllowed = car.IsChildTariffTypeAllowed;
            newDbCar.IsForDisabledPersons = car.IsForDisabledPersons;
            newDbCar.IsMealOptionPossible = car.IsMealOptionPossible;
            newDbCar.IsOnRequestMealOptionPossible = car.IsOnRequestMealOptionPossible;
            newDbCar.IsTwoStorey = car.IsTwoStorey;
            newDbCar.LocalArrivalDateTime = car.LocalArrivalDateTime.ToUniversalTime();
            newDbCar.MaxPrice = car.MaxPrice;
            newDbCar.MealSalesOpenedTill = car.MealSalesOpenedTill.ToUniversalTime();
            newDbCar.MinPrice = car.MinPrice;
            newDbCar.OnlyNonRefundableTariff = car.OnlyNonRefundableTariff;
            newDbCar.PassengerSpecifyingRules = car.PassengerSpecifyingRules;
            newDbCar.PlaceQuantity = car.PlaceQuantity;
            newDbCar.PlaceReservationType = car.PlaceReservationType;
            newDbCar.PlacesWithConditionalRefundableTariffQuantity = car.PlacesWithConditionalRefundableTariffQuantity;
            newDbCar.ServiceClass = car.ServiceClass;
            newDbCar.ServiceCost = car.ServiceCost;
            newDbCar.Services = car.Services.ToList();
            newDbCar.TripDirection = car.TripDirection;
        }

        private Dictionary<string, string> GetChangesFromCar(Car dbCar, RzdCar car)
        {
            var changes = new Dictionary<string, string>();

            if (dbCar.ArePlacesForBusinessTravelBooking != car.ArePlacesForBusinessTravelBooking)
                changes[nameof(dbCar.ArePlacesForBusinessTravelBooking)] = JsonSerializer.Serialize(dbCar.ArePlacesForBusinessTravelBooking);

            if (dbCar.ArrivalDateTime != car.ArrivalDateTime)
                changes[nameof(dbCar.ArrivalDateTime)] = JsonSerializer.Serialize(dbCar.ArrivalDateTime.ToUniversalTime());

            if (dbCar.AvailabilityIndication != car.AvailabilityIndication)
                changes[nameof(dbCar.AvailabilityIndication)] = dbCar.AvailabilityIndication;

            if (dbCar.CarNumber != car.CarNumber)
                changes[nameof(dbCar.CarNumber)] = dbCar.CarNumber;

            if (dbCar.CarPlaceType != car.CarPlaceType)
                changes[nameof(dbCar.CarPlaceType)] = dbCar.CarPlaceType;

            if (dbCar.CarSubType != car.CarSubType)
                changes[nameof(dbCar.CarSubType)] = dbCar.CarSubType;

            if (dbCar.CarType != car.CarType)
                changes[nameof(dbCar.CarType)] = dbCar.CarType;

            if (dbCar.DestinationStationCode != car.DestinationStationCode)
                changes[nameof(dbCar.DestinationStationCode)] = dbCar.DestinationStationCode;

            if (dbCar.FreePlaces != car.FreePlaces)
                changes[nameof(dbCar.FreePlaces)] = dbCar.FreePlaces;

            if (dbCar.MaxPrice != car.MaxPrice)
                changes[nameof(dbCar.MaxPrice)] = JsonSerializer.Serialize(dbCar.MaxPrice);

            if (dbCar.MinPrice != car.MinPrice)
                changes[nameof(dbCar.MinPrice)] = JsonSerializer.Serialize(dbCar.MinPrice);

            if (dbCar.PlaceQuantity != car.PlaceQuantity)
                changes[nameof(dbCar.PlaceQuantity)] = JsonSerializer.Serialize(dbCar.PlaceQuantity);

            if (dbCar.ServiceClass != car.ServiceClass)
                changes[nameof(dbCar.ServiceClass)] = dbCar.ServiceClass;

            if (dbCar.ServiceCost != car.ServiceCost)
                changes[nameof(dbCar.ServiceCost)] = JsonSerializer.Serialize(dbCar.ServiceCost);

            if (!dbCar.Services.OrderBy(x => x).SequenceEqual(car.Services.OrderBy(x => x)))
                changes[nameof(dbCar.Services)] = JsonSerializer.Serialize(dbCar.Services);

            if (dbCar.TripDirection != car.TripDirection)
                changes[nameof(dbCar.TripDirection)] = dbCar.TripDirection;

            return changes;
        }

        private async Task UpdateDbTrainAsync(Train dbTrain, RzdTrain train)
        {
            dbTrain.CreatedDate = DateTimeOffset.Now.ToUniversalTime();
            dbTrain.ArrivalDateTime = train.ArrivalDateTime.ToUniversalTime();
            dbTrain.ArrivalStopTime = train.ArrivalStopTime;
            dbTrain.CarServices = train.CarServices;
            dbTrain.DepartureDateTime = train.DepartureDateTime.ToUniversalTime();
            dbTrain.DepartureStopTime = train.DepartureStopTime;
            dbTrain.DisplayTrainNumber = train.DisplayTrainNumber;
            dbTrain.HasCarTransportationCoaches = train.HasCarTransportationCoaches;
            dbTrain.HasDynamicPricingCars = train.HasDynamicPricingCars;
            dbTrain.HasTwoStoreyCars = train.HasTwoStoreyCars;
            dbTrain.InitialTrainStationCode = train.InitialTrainStationCode;
            dbTrain.IsBranded = train.IsBranded;
            dbTrain.IsFromSchedule = train.IsFromSchedule;
            dbTrain.IsPlaceRangeAllowed = train.IsPlaceRangeAllowed;
            dbTrain.IsSaleForbidden = train.IsSaleForbidden;
            dbTrain.IsSuburban = train.IsSuburban;
            dbTrain.IsTicketPrintRequiredForBoarding = train.IsTicketPrintRequiredForBoarding;
            dbTrain.IsTourPackagePossible = train.IsTourPackagePossible;
            dbTrain.IsTrainRouteAllowed = train.IsTrainRouteAllowed;
            dbTrain.IsWaitListAvailable = train.IsWaitListAvailable;
            dbTrain.LocalArrivalDateTime = train.LocalArrivalDateTime.ToUniversalTime();
            dbTrain.LocalDepartureDateTime = train.LocalDepartureDateTime.ToUniversalTime();
            dbTrain.TrainBrandCode = train.TrainBrandCode;
            dbTrain.TrainDescription = train.TrainDescription;
            dbTrain.TrainNumber = train.TrainNumber;
            dbTrain.TrainNumberToGetRoute = train.TrainNumberToGetRoute;
            dbTrain.TripDistance = train.TripDistance;
            dbTrain.TripDuration = train.TripDuration;
            dbTrain.OriginStationCode = train.OriginStationCode;
            dbTrain.DestinationStationCode = train.DestinationStationCode;
            dbTrain.FinalTrainStationCode = train.FinalTrainStationCode;

        }

        private Dictionary<string, string> GetChangesFromTrain(Train train, RzdTrain newDbTrain)
        {
            var changes = new Dictionary<string, string>();

            if (newDbTrain.ArrivalDateTime != train.ArrivalDateTime)
                changes["ArrivalDateTime"] = JsonSerializer.Serialize(train.ArrivalDateTime.ToUniversalTime());

            if (newDbTrain.ArrivalStopTime != train.ArrivalStopTime)
                changes["ArrivalStopTime"] = JsonSerializer.Serialize(train.ArrivalStopTime);

            if (!newDbTrain.CarServices.OrderBy(x => x).SequenceEqual(train.CarServices.OrderBy(x => x)))
                changes["CarServices"] = JsonSerializer.Serialize(train.CarServices);

            if (newDbTrain.DepartureDateTime != train.DepartureDateTime)
                changes["DepartureDateTime"] = JsonSerializer.Serialize(train.DepartureDateTime.ToUniversalTime());

            if (newDbTrain.DepartureStopTime != train.DepartureStopTime)
                changes["DepartureStopTime"] = JsonSerializer.Serialize(train.DepartureStopTime);

            if (newDbTrain.DisplayTrainNumber != train.DisplayTrainNumber)
                changes["DisplayTrainNumber"] = train.DisplayTrainNumber;

            if (newDbTrain.HasCarTransportationCoaches != train.HasCarTransportationCoaches)
                changes["HasCarTransportationCoaches"] = JsonSerializer.Serialize(train.HasCarTransportationCoaches);

            if (newDbTrain.HasDynamicPricingCars != train.HasDynamicPricingCars)
                changes["HasDynamicPricingCars"] = JsonSerializer.Serialize(train.HasDynamicPricingCars);

            if (newDbTrain.HasTwoStoreyCars != train.HasTwoStoreyCars)
                changes["HasTwoStoreyCars"] = JsonSerializer.Serialize(train.HasTwoStoreyCars);

            if (newDbTrain.InitialTrainStationCode != train.InitialTrainStationCode)
                changes["InitialTrainStationCode"] = train.InitialTrainStationCode;

            if (newDbTrain.IsBranded != train.IsBranded)
                changes["IsBranded"] = JsonSerializer.Serialize(train.IsBranded);

            if (newDbTrain.IsFromSchedule != train.IsFromSchedule)
                changes["IsFromSchedule"] = JsonSerializer.Serialize(train.IsFromSchedule);

            if (newDbTrain.IsPlaceRangeAllowed != train.IsPlaceRangeAllowed)
                changes["IsPlaceRangeAllowed"] = JsonSerializer.Serialize(train.IsPlaceRangeAllowed);

            if (newDbTrain.IsSaleForbidden != train.IsSaleForbidden)
                changes["IsSaleForbidden"] = JsonSerializer.Serialize(train.IsSaleForbidden);

            if (newDbTrain.IsSuburban != train.IsSuburban)
                changes["IsSuburban"] = JsonSerializer.Serialize(train.IsSuburban);

            if (newDbTrain.IsTicketPrintRequiredForBoarding != train.IsTicketPrintRequiredForBoarding)
                changes["IsTicketPrintRequiredForBoarding"] = JsonSerializer.Serialize(train.IsTicketPrintRequiredForBoarding);

            if (newDbTrain.IsTourPackagePossible != train.IsTourPackagePossible)
                changes["IsTourPackagePossible"] = JsonSerializer.Serialize(train.IsTourPackagePossible);

            if (newDbTrain.IsTrainRouteAllowed != train.IsTrainRouteAllowed)
                changes["IsTrainRouteAllowed"] = JsonSerializer.Serialize(train.IsTrainRouteAllowed);

            if (newDbTrain.IsWaitListAvailable != train.IsWaitListAvailable)
                changes["IsWaitListAvailable"] = JsonSerializer.Serialize(train.IsWaitListAvailable);

            if (newDbTrain.LocalArrivalDateTime != train.LocalArrivalDateTime)
                changes["LocalArrivalDateTime"] = JsonSerializer.Serialize(train.LocalArrivalDateTime.ToUniversalTime());

            if (newDbTrain.LocalDepartureDateTime != train.LocalDepartureDateTime)
                changes["LocalDepartureDateTime"] = JsonSerializer.Serialize(train.LocalDepartureDateTime.ToUniversalTime());

            if (newDbTrain.TrainBrandCode != train.TrainBrandCode)
                changes["TrainBrandCode"] = train.TrainBrandCode;

            if (newDbTrain.TrainDescription != train.TrainDescription)
                changes["TrainDescription"] = train.TrainDescription;

            if (newDbTrain.TrainNumber != train.TrainNumber)
                changes["TrainNumber"] = train.TrainNumber;

            if (newDbTrain.TrainNumberToGetRoute != train.TrainNumberToGetRoute)
                changes["TrainNumberToGetRoute"] = train.TrainNumberToGetRoute;

            if (newDbTrain.TripDistance != train.TripDistance)
                changes["TripDistance"] = JsonSerializer.Serialize(train.TripDistance);

            if (newDbTrain.TripDuration != train.TripDuration)
                changes["TripDuration"] = JsonSerializer.Serialize(train.TripDuration);

            return changes;
        }
    }
}
