using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using RZD.Application.Helpers;
using RZD.Common.Configs;
using RZD.Common.Enums;
using RZD.Database;
using RZD.Database.Models;
using RZD.Integration;
using RZD.Integration.Models.CarPricing;
using RZD.Integration.Models.TrainPricing;
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
            var dtStart = DateTimeOffset.Now;

            var trackedRoutes = _ctx.TrackedRoutes.Where(x => !x.IsDeleted).ToList();

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
                            var a = train.CarGroups;
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

                                    
                                }
                            }
                            await _ctx.SaveChangesAsync();

                            if (train.CarGroups.Any())
                            {
                                await RefreshCarPlacesAsync(train.OriginStationCode, train.DestinationStationCode, train.DepartureDateTime.DateTime, train.TrainNumber, dbTrain.Id);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await LogErrorToDbAsync("RefreshTrainsAsync", ex);
                        logger.LogError(ex, "Error in RefreshTrainsAsync");
                    }
                }
            }
            var dtFinish = DateTimeOffset.Now;
            var timeTaken = dtFinish - dtStart;
            await _ctx.Statistics.AddAsync(new Statistic
            {
                Name = "RefreshTrainsAsync",
                Comment = $"Finished RefreshTrainsAsync. Time taken: {timeTaken}",
                IsSuccess = true,
                DateStart = dtStart.UtcDateTime,
                DateFinish = dtFinish.UtcDateTime,
            });
            await _ctx.SaveChangesAsync();

            logger.LogInformation($"Finished RefreshTrainsAsync. Time taken: {timeTaken}");
        }

        public async Task RefreshCarPlacesAsync(string origin, string destination, DateTime departureDate, string trainNumber, long trainId)
        {
            logger.LogInformation($"RefreshCarPlacesAsync for origin:{origin}, destination:{destination}, departureDate:{departureDate}, trainNumber:{trainNumber}");
            try
            {

                #region Сохраняем изменение флага IsBooked
                var carsResponse = await _api.CarPricingAsync(origin, destination, departureDate, trainNumber);

                var dbCarPlaces = await _ctx.CarPlaces
                    .Where(x => x.TrainId == trainId).ToListAsync();

                foreach (var dbCarPlace in dbCarPlaces)
                {
                    var carPlaceNumbers = carsResponse.Cars
                        .Where(x => dbCarPlace.CarNumber == dbCarPlace.CarNumber)
                        .SelectMany(x => x.FreePlaces.Split(",").Select(y => y.Trim())).ToList();

                    if (dbCarPlace.IsFree && !carPlaceNumbers.Any(x => x == dbCarPlace.CarPlaceNumber))
                    {
                        await _ctx.EntityHistories.AddAsync(new EntityHistory()
                        {
                            EntityTypeId = (int)EntityTypes.CarPlace,
                            EntityId = dbCarPlace.Id,
                            ChangedAt = DateTimeOffset.UtcNow,
                            FieldName = nameof(dbCarPlace.IsFree),
                            OldFieldValue = JsonSerializer.Serialize(dbCarPlace.IsFree)
                        });


                        dbCarPlace.IsFree = false;
                    }
                    else if(!dbCarPlace.IsFree && carPlaceNumbers.Any(x => x == dbCarPlace.CarPlaceNumber))
                    {
                        await _ctx.EntityHistories.AddAsync(new EntityHistory()
                        {
                            EntityTypeId = (int)EntityTypes.CarPlace,
                            EntityId = dbCarPlace.Id,
                            ChangedAt = DateTimeOffset.UtcNow,
                            FieldName = nameof(dbCarPlace.IsFree),
                            OldFieldValue = JsonSerializer.Serialize(dbCarPlace.IsFree)
                        });


                        dbCarPlace.IsFree = true;
                    }
                }

                await _ctx.SaveChangesAsync();

                #endregion

                foreach (var car in carsResponse.Cars)
                {
                    var placeNumbers = car.FreePlaces.Split(",").Select(x => x.Trim()).ToList();

                    foreach (var placeNumber in placeNumbers)
                    {
                        if (string.IsNullOrWhiteSpace(placeNumber))
                        {
                            continue;
                        }

                        var dbCarPlace = await _ctx.CarPlaces
                                .Where(x => x.TrainId == trainId
                                    && x.CarNumber == car.CarNumber
                                    && x.CarPlaceNumber == placeNumber)
                                .FirstOrDefaultAsync();

                        if (dbCarPlace == null)
                        {
                            var carPlace = new CarPlace()
                            {
                                CarPlaceNumber = placeNumber,
                                IsFree = true,
                                TrainId = trainId,
                                CreatedDate = DateTimeOffset.UtcNow,
                            };
                            UpdateDbCarPlace(carPlace, car);

                            await _ctx.CarPlaces.AddAsync(carPlace);
                        }
                        else
                        {
                            var changes = GetChangesFromCar(dbCarPlace, car);
                            if (changes.Any())
                            {
                                UpdateDbCarPlace(dbCarPlace, car);

                                foreach (var change in changes)
                                {
                                    var entityHistory = new EntityHistory
                                    {
                                        ChangedAt = DateTimeOffset.Now.ToUniversalTime(),
                                        FieldName = change.Key,
                                        OldFieldValue = change.Value,
                                        EntityId = dbCarPlace.Id,
                                        EntityTypeId = (int)EntityTypes.CarPlace,
                                    };

                                    await _ctx.EntityHistories.AddAsync(entityHistory);
                                }
                            }
                        }
                    }

                }

                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await LogErrorToDbAsync("RefreshCarPlacesAsync", ex);
                logger.LogError(ex, "Error in RefreshCarPlacesAsync");
            }
        }

        private void UpdateDbCarPlace(CarPlace carPlace, RzdCar car)
        {
            carPlace.ArePlacesForBusinessTravelBooking = car.ArePlacesForBusinessTravelBooking;
            carPlace.ArrivalDateTime = car.ArrivalDateTime.ToUniversalTime();
            carPlace.AvailabilityIndication = car.AvailabilityIndication;
            carPlace.CarNumber = car.CarNumber;
            carPlace.CarPlaceType = car.CarPlaceType;
            carPlace.CarSubType = car.CarSubType;
            carPlace.CarType = car.CarType;
            carPlace.DestinationStationCode = car.DestinationStationCode;
            carPlace.HasDynamicPricing = car.HasDynamicPricing;
            carPlace.HasFssBenefit = car.HasFssBenefit;
            carPlace.HasGenderCabins = car.HasGenderCabins;
            carPlace.HasNonRefundableTariff = car.HasNonRefundableTariff;
            carPlace.HasPlacesNearBabies = car.HasPlacesNearBabies;
            carPlace.HasPlacesNearPets = car.HasPlacesNearPets;
            carPlace.IsAdditionalMealOptionPossible = car.IsAdditionalMealOptionPossible;
            carPlace.IsAdditionalPassengerAllowed = car.IsAdditionalPassengerAllowed;
            carPlace.IsBranded = car.IsBranded;
            carPlace.IsBuffet = car.IsBuffet;
            carPlace.IsCarTransportationCoach = car.IsCarTransportationCoach;
            carPlace.IsChildTariffTypeAllowed = car.IsChildTariffTypeAllowed;
            carPlace.IsForDisabledPersons = car.IsForDisabledPersons;
            carPlace.IsMealOptionPossible = car.IsMealOptionPossible;
            carPlace.IsOnRequestMealOptionPossible = car.IsOnRequestMealOptionPossible;
            carPlace.IsTwoStorey = car.IsTwoStorey;
            carPlace.LocalArrivalDateTime = car.LocalArrivalDateTime.ToUniversalTime();
            carPlace.MaxPrice = car.MaxPrice;
            carPlace.MealSalesOpenedTill = car.MealSalesOpenedTill.ToUniversalTime();
            carPlace.MinPrice = car.MinPrice;
            carPlace.OnlyNonRefundableTariff = car.OnlyNonRefundableTariff;
            carPlace.PassengerSpecifyingRules = car.PassengerSpecifyingRules;
            carPlace.PlaceReservationType = car.PlaceReservationType;
            carPlace.PlacesWithConditionalRefundableTariffQuantity = car.PlacesWithConditionalRefundableTariffQuantity;
            carPlace.ServiceClass = car.ServiceClass;
            carPlace.ServiceCost = car.ServiceCost;
            carPlace.Services = car.Services.ToList();
            carPlace.TripDirection = car.TripDirection;
        }

        private Dictionary<string, string?> GetChangesFromCar(CarPlace dbCarPlace, RzdCar rzdCar)
        {
            var changes = new Dictionary<string, string?>();

            if (dbCarPlace.ArePlacesForBusinessTravelBooking != rzdCar.ArePlacesForBusinessTravelBooking)
                changes[nameof(dbCarPlace.ArePlacesForBusinessTravelBooking)] = JsonSerializer.Serialize(dbCarPlace.ArePlacesForBusinessTravelBooking);

            if (dbCarPlace.ArrivalDateTime != rzdCar.ArrivalDateTime)
                changes[nameof(dbCarPlace.ArrivalDateTime)] = JsonSerializer.Serialize(dbCarPlace.ArrivalDateTime.ToUniversalTime());

            if (dbCarPlace.AvailabilityIndication != rzdCar.AvailabilityIndication)
                changes[nameof(dbCarPlace.AvailabilityIndication)] = dbCarPlace.AvailabilityIndication;

            if (dbCarPlace.CarNumber != rzdCar.CarNumber)
                changes[nameof(dbCarPlace.CarNumber)] = dbCarPlace.CarNumber;

            if (dbCarPlace.CarPlaceType != rzdCar.CarPlaceType)
                changes[nameof(dbCarPlace.CarPlaceType)] = dbCarPlace.CarPlaceType;

            if (dbCarPlace.CarSubType != rzdCar.CarSubType)
                changes[nameof(dbCarPlace.CarSubType)] = dbCarPlace.CarSubType;

            if (dbCarPlace.CarType != rzdCar.CarType)
                changes[nameof(dbCarPlace.CarType)] = dbCarPlace.CarType;

            if (dbCarPlace.DestinationStationCode != rzdCar.DestinationStationCode)
                changes[nameof(dbCarPlace.DestinationStationCode)] = dbCarPlace.DestinationStationCode;

            if (dbCarPlace.MaxPrice != rzdCar.MaxPrice)
                changes[nameof(dbCarPlace.MaxPrice)] = JsonSerializer.Serialize(dbCarPlace.MaxPrice);

            if (dbCarPlace.MinPrice != rzdCar.MinPrice)
                changes[nameof(dbCarPlace.MinPrice)] = JsonSerializer.Serialize(dbCarPlace.MinPrice);


            if (dbCarPlace.ServiceClass != rzdCar.ServiceClass)
                changes[nameof(dbCarPlace.ServiceClass)] = dbCarPlace.ServiceClass;

            if (dbCarPlace.ServiceCost != rzdCar.ServiceCost)
                changes[nameof(dbCarPlace.ServiceCost)] = JsonSerializer.Serialize(dbCarPlace.ServiceCost);

            if (!dbCarPlace.Services.OrderBy(x => x).SequenceEqual(rzdCar.Services.OrderBy(x => x)))
                changes[nameof(dbCarPlace.Services)] = JsonSerializer.Serialize(dbCarPlace.Services);

            if (dbCarPlace.TripDirection != rzdCar.TripDirection)
                changes[nameof(dbCarPlace.TripDirection)] = dbCarPlace.TripDirection;

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

        private async Task LogErrorToDbAsync(string name, Exception ex)
        {
            await _ctx.Statistics.AddAsync(new Statistic
            {
                Name = name,
                Comment = $"Error in {name}: ex: {ex.Message}, inex: {ex.InnerException?.Message}, stackTrace: {ex.StackTrace}",
                IsSuccess = false,
                DateStart = DateTimeOffset.UtcNow,
            });
            await _ctx.SaveChangesAsync();
        }
    }
}
