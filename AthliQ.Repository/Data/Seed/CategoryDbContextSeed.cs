using AthliQ.Core.Entities.Models;
using AthliQ.Repository.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AthliQ.Repository.Data.Seed
{
    public static class CategoryDbContextSeed
    {
        public static async Task SeedCategoryAsync(AthliQDbContext dbContext)
        {
            if (!await dbContext.Categories.AnyAsync())
            {
                var jsonData = File.ReadAllText("../AthliQ.Repository/Data/DataSeeding/Categories.json");
                var categories = JsonSerializer.Deserialize<List<Category>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (categories?.Count > 0)
                {
                    await dbContext.Categories.AddRangeAsync(categories);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
