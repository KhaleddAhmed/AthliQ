using System.Text;
using AthliQ.Core;
using AthliQ.Core.Entities;
using AthliQ.Core.Mapping;
using AthliQ.Core.Service.Contract;
using AthliQ.Repository;
using AthliQ.Repository.Data.Contexts;
using AthliQ.Repository.Data.Seed;
using AthliQ.Service.Services.Cache;
using AthliQ.Service.Services.Categories;
using AthliQ.Service.Services.Children;
using AthliQ.Service.Services.Sports;
using AthliQ.Service.Services.Tests;
using AthliQ.Service.Services.Token;
using AthliQ.Service.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace AthliQ.User.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configure Services
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ITestService, TestService>();
            builder.Services.AddScoped<ISportService, SportService>();
            builder.Services.AddScoped<IChildService, ChildService>();
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddHttpClient<IChildService, ChildService>();

            builder.Services.AddDbContext<AthliQDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("AthliQConnection"));
            });
            builder.Services.AddSingleton<IConnectionMultiplexer>(
                (serviceProvider) =>
                {
                    var connection = builder.Configuration.GetConnectionString("Redis");

                    return ConnectionMultiplexer.Connect(connection);
                }
            );
            builder
                .Services.AddIdentity<AthliQUser, IdentityRole>()
                .AddEntityFrameworkStores<AthliQDbContext>();

            builder
                .Services.AddAuthentication(option =>
                {
                    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                        {
                            ValidateIssuer = true,
                            ValidIssuer = builder.Configuration["JWT:Issuer"],
                            ValidateAudience = true,
                            ValidAudience = builder.Configuration["JWT:Audience"],
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])
                            ),
                        };
                });

            builder.Services.AddCors(o =>
            {
                o.AddPolicy(
                    "MyCors",
                    c =>
                    {
                        c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                    }
                );
            });
            #endregion

            var app = builder.Build();

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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("MyCors");

            app.MapControllers();

            app.Run();
        }
    }
}
