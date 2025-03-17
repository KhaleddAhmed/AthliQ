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
    public static class TestDbContextSeed
    {
        public static async Task SeedTestAsync(AthliQDbContext dbContext)
        {
            if (!await dbContext.Tests.AnyAsync())
            {
                var jsonData = File.ReadAllText("../AthliQ.Repository/Data/DataSeeding/Tests.json");
                var tests = JsonSerializer.Deserialize<List<Test>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (tests?.Count > 0)
                {
                    foreach (var test in tests)
                    {

                        await dbContext.Tests.AddAsync(test);
                    }
                    var row = await dbContext.SaveChangesAsync();
                    if (row > 0)
                        Console.WriteLine("returned");
                }
            }
        }
    }
}
