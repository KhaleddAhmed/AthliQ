using AthliQ.Core.Mapping;
using AthliQ.Core.Service.Contract;
using AthliQ.Repository;
using AthliQ.Service;
using AthliQ.Service.Helpers;
using AthliQ.Service.Services.Mail;
using AthliQ.User.API.Extensions;

namespace AthliQ.User.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Repository (Infrastructure) Layer Services
            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            //Service Layer Services
            builder.Services.AddApplicationServices();

            //Web Application (API) Layer Services
            builder.Services.AddWebApplicationServices();

            //JWT Services
            builder.Services.AddJWTServices(builder.Configuration);
            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings")
            );
            builder.Services.AddTransient<IEmailService, EmailService>();

            #endregion

            var app = builder.Build();

            await app.SeedDatabaseAsync();

            #region Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("MyCors");

            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}
