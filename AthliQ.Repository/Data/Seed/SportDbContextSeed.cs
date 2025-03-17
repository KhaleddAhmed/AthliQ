using AthliQ.Core.Entities.Models;
using AthliQ.Repository.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AthliQ.Repository.Data.Seed
{
    public static class SportDbContextSeed
    {
        public static async Task SeedSportAsync(AthliQDbContext dbContext)
        {
            if (!await dbContext.Sports.AnyAsync())
            {
                var jsonData = File.ReadAllText("../AthliQ.Repository/Data/DataSeeding/Sports.json");
                var sports = JsonSerializer.Deserialize<List<Sport>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (sports?.Count > 0)
                {
                    foreach (var sport in sports)
                    {

                    await dbContext.Sports.AddAsync(sport);
                    }
                    var row = await dbContext.SaveChangesAsync();
                    if(row>0)
                        Console.WriteLine("returned");
                }
            }
        }
    }
}
