using AthliQ.Core;
using AthliQ.Repository.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AthliQ.Repository
{
	public static class InfrastructureServicesRegisteration
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection Services, IConfiguration Configuration)
		{
			Services.AddScoped<IUnitOfWork, UnitOfWork>();

			Services.AddDbContext<AthliQDbContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("AthliQConnection"));
			});

			Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
			{
				var connection = Configuration.GetConnectionString("Redis");
				return ConnectionMultiplexer.Connect(connection);
			});

			return Services;
		}
	}
}
