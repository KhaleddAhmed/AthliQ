using AthliQ.Core.Entities;
using AthliQ.Repository.Data.Contexts;
using AthliQ.Repository.Data.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AthliQ.User.API.Extensions
{
	public static class WebApplicationRegisteration
	{
		public static async Task SeedDatabaseAsync(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();

			var services = scope.ServiceProvider;

			var _dbContext = services.GetRequiredService<AthliQDbContext>();
			var loggerFactory = services.GetRequiredService<ILoggerFactory>();
			var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = services.GetRequiredService<UserManager<AthliQUser>>();

			try
			{
				await _dbContext.Database.MigrateAsync();
				await RoleDbContextSeed.SeedRoleAsync(roleManager);
				await AdminDbContextSeed.SeedAdminAsync(userManager);

				await CategoryDbContextSeed.SeedCategoryAsync(_dbContext);
				await SportDbContextSeed.SeedSportAsync(_dbContext);
				await TestDbContextSeed.SeedTestAsync(_dbContext);
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<Program>();
				logger.LogError(ex, "An Error Occured During Apply The Migration");
			}
		}
	}
}
