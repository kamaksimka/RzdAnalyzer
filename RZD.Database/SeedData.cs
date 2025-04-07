using RZD.Common.Enums;
using RZD.Database.Models;

namespace RZD.Database
{
    public class SeedData
    {
        public static async Task SeedAsync(DataContext context)
        {
            if (!context.EntityTypes.Any())
            {
                await context.EntityTypes.AddRangeAsync(new[]
                {
                    new EntityType
                    {
                        Id = (int)EntityTypes.Train,
                        Name = EntityTypes.Train.ToString()
                    },
                    new EntityType
                    {
                        Id = (int)EntityTypes.CarPlace,
                        Name = EntityTypes.CarPlace.ToString()
                    }
                });
            }

            if (!context.Roles.Any())
            {
                await context.Roles.AddRangeAsync(new[]
                {
                    new Role
                    {
                        Id = (int)Roles.Admin,
                        Name = Roles.Admin.ToString()
                    }
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
