using Microsoft.EntityFrameworkCore;
using RZD.Common.Configs;
using RZD.Database.Models;
using System;
using System.Collections.Generic;


namespace RZD.Database
{
    public class DataContext : DbContext
    {
        private readonly RzdConfig _config;

        public DbSet<Car> Cars { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<FreePlacesByCompartment> FreePlacesByCompartment { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<ServicesWithIndication> ServicesWithIndication { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<TrainStation> TrainStations { get; set; }

        public DataContext(RzdConfig config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_config.ConnectionString).UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
