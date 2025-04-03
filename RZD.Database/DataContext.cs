using Microsoft.EntityFrameworkCore;
using RZD.Common.Configs;
using RZD.Database.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Npgsql;


namespace RZD.Database
{
    public class DataContext : DbContext
    {
        private readonly RzdConfig _config;

        public DbSet<Car> Cars { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<TrainStation> TrainStations { get; set; }
        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<EntityHistory> EntityHistories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<TrackedRoute> TrackedRoutes { get; set; }

        public DataContext(IOptions<RzdConfig> config)
        {
            _config = config.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_config.ConnectionString)
                .UseSnakeCaseNamingConvention();

            NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(x => x.Roles)
                .WithMany(x => x.Users)
                .UsingEntity<UserRole>();

            modelBuilder.Entity<RouteStop>()
                .HasOne(x => x.Route)
                .WithMany(x => x.RouteStops)
                .HasForeignKey(x => x.RouteId);

            modelBuilder.Entity<City>()
                .HasIndex(x => x.NodeId).IsUnique();

            modelBuilder.Entity<TrainStation>()
                .HasIndex(x => x.NodeId).IsUnique();

            modelBuilder.Entity<Train>()
                .HasIndex(x => new
                {
                    x.TrainNumber,
                    x.OriginStationCode,
                    x.DestinationStationCode,
                    x.DepartureDateTime
                }).IsUnique();

            modelBuilder.Entity<Car>()
                .HasIndex(x => new
                {
                    x.TrainId,
                    x.CarNumber,
                    x.CarPlaceType,
                    x.CarType,
                    x.CarSubType,
                }).IsUnique();

        }
    }
}
