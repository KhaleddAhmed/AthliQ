using AthliQ.Core.Service.Contract;
using AthliQ.Service.Services.Admin;
using AthliQ.Service.Services.Cache;
using AthliQ.Service.Services.Categories;
using AthliQ.Service.Services.Children;
using AthliQ.Service.Services.Report;
using AthliQ.Service.Services.Sports;
using AthliQ.Service.Services.Tests;
using AthliQ.Service.Services.Token;
using AthliQ.Service.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace AthliQ.Service
{
    public static class ApplicationServicesRegisteration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {
            Services.AddScoped<IUserService, UserService>();
            Services.AddScoped<ITokenService, TokenService>();
            Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<ITestService, TestService>();
            Services.AddScoped<ISportService, SportService>();
            Services.AddScoped<IChildService, ChildService>();
            Services.AddScoped<ICacheService, CacheService>();
            //Services.AddHttpClient<IChildService, ChildService>();
            Services.AddScoped<IAdminService, AdminService>();

            Services.AddScoped<IReportGenerationService, ReportService>();
            return Services;
        }
    }
}
