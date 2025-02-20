using AthliQ.Core.Entities;
using AthliQ.Repository.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AthliQ.User.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AthliQDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("AthliQConnection"));
            });

            builder
                .Services.AddIdentity<AthliQUser, IdentityRole>()
                .AddEntityFrameworkStores<AthliQDbContext>();

            var app = builder.Build();

            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _dbContext = services.GetRequiredService<AthliQDbContext>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                await _dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error Occured During Apply The Migration");
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
