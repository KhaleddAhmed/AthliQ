using AthliQ.Core.Mapping;
using AthliQ.Core.Repository.Contract;
using AthliQ.Core.Service.Contract;
using AthliQ.Repository;
using AthliQ.Repository.BodyImageAnalysis;
using AthliQ.Repository.RuleEngine;
using AthliQ.Service;
using AthliQ.Service.Helpers;
using AthliQ.Service.Services.Mail;
using AthliQ.User.API.CustomMiddleWares;
using AthliQ.User.API.Extensions;
using Hangfire;

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
            builder.Services.Configure<SmtpMailSettings>(
                builder.Configuration.GetSection("MailSettings")
            );
            builder.Services.AddTransient<IEmailService, EmailService>();

            builder.Services.AddHttpClient<ICategoryEvaluationService, DroolsCategoryEvaluationService>();
            builder.Services.Configure<CategoryEvaluationOptions>(builder.Configuration.GetSection("CategoryEvaluationSettings"));

            builder.Services.Configure<BodyImageAnalysisOptions>(builder.Configuration.GetSection("BodyImageAnalysisSettings"));
            builder.Services.AddHttpClient<IBodyImageAnalysisService, BodyImageAnalysisService>();

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
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("MyCors");

            app.UseHangfireDashboard("/hangfire");

            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}
