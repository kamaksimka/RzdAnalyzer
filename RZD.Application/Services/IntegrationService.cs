using Microsoft.EntityFrameworkCore;
using RZD.API;
using RZD.Application.Helpers;
using RZD.Database;
using RZD.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Services
{
    public class IntegrationService
    {
        private readonly DataContext _ctx;
        private readonly RzdApi _api;

        public IntegrationService(DataContext ctx, RzdApi api)
        {
            _ctx = ctx;
            _api = api;
        }

        public async Task RefreshCitiesAsync()
        {
            foreach (var ch in AlphabetHelper.RussianAlphabet)
            {
                var suggests = await _api.SuggestsAsync(ch);

                #region create-update Cities

                var cities = suggests.City;

                var dbCities = await _ctx.Cities
                    .Where(x => cities.Select(y => y.ExpressCode).Contains(x.ExpressCode))
                    .ToListAsync();

                var newDbCities = cities
                    .Where(x => !dbCities.Any(y => y.ExpressCode == x.ExpressCode))
                    .Select(x => new City
                    {
                        ExpressCode = x.ExpressCode,
                        ExpressCodes = x.ExpressCodes,
                        ForeignCode = x.ForeignCode,
                        Name = x.Name,
                        Region = x.Region,
                    }).ToList();

                await _ctx.Cities.AddRangeAsync(newDbCities);
                
                await _ctx.SaveChangesAsync();

                #endregion

                #region create-update Trains

                var trainStations = suggests.Train;

                var dbTrainStations = await _ctx.TrainStations
                    .Where(x => trainStations.Select(y => y.ExpressCode).Contains(x.ExpressCode))
                    .ToListAsync();

                var newDbTrainStations = trainStations
                    .Where(x => !dbTrainStations.Any(y => y.ExpressCode == x.ExpressCode))
                    .Select(x => new TrainStation
                    {
                        ExpressCode = x.ExpressCode,
                        ForeignCode = x.ForeignCode,
                        Name = x.Name,
                        Region = x.Region
                    }).ToList();

                await _ctx.TrainStations.AddRangeAsync(newDbTrainStations);

                await _ctx.SaveChangesAsync();

                #endregion
            }
        }
    }
}
