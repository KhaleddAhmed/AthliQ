using AthliQ.Core.Entities;
using AthliQ.Core.Service.Contract;
using AthliQ.Repository.Data.Contexts;
using AthliQ.Service.Services.Children;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AthliQ.User.API.Extensions
{
	public static class ServicesRegisteration
	{
		public static IServiceCollection AddWebApplicationServices(this IServiceCollection Services)
		{
			Services.AddHttpClient<IChildService, ChildService>();

			Services.AddIdentity<AthliQUser, IdentityRole>()
							.AddEntityFrameworkStores<AthliQDbContext>();

			Services.AddCors(o =>
			{
				o.AddPolicy("MyCors", c =>
					{
						c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
					}
				);
			});
			return Services;
		}

		public static IServiceCollection AddJWTServices(this IServiceCollection Services, IConfiguration Configuration)
		{
			Services.AddAuthentication(option =>
			{
				option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidIssuer = Configuration["JWT:Issuer"],
					ValidateAudience = true,
					ValidAudience = Configuration["JWT:Audience"],
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
				};
			});

			return Services;
		}
	}
}
