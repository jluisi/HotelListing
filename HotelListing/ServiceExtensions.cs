using HotelListing.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Text;

namespace HotelListing
{
  public static class ServiceExtensions
  {
    public static void ConfigureIdentity(this IServiceCollection services)
    {
      var builder = services.AddIdentityCore<ApiUser>(q => q.User.RequireUniqueEmail = true);
      builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
      builder.AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
    }

    //---------------------------------------------------------------------------------------------
    // IConfiguration instance gives us access to the appsettings.json configuration file
    //---------------------------------------------------------------------------------------------
    public static void ConfigureJWT(this IServiceCollection services, IConfiguration Configuration)
    {
      var jwtSettings = Configuration.GetSection("Jwt");
      var key = Environment.GetEnvironmentVariable("KEY");

      services
        .AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
          // This is just a guide, it can change according with our own needs
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,    // KEY Environment Variable
            ValidIssuer = jwtSettings.GetSection("Issuer").Value,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
          };
        });
    }

    //---------------------------------------------------------------------------------------------
    // Custom Global Exception Handler
    //---------------------------------------------------------------------------------------------
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
      app.UseExceptionHandler(error =>
      {
        error.Run(async context =>
        {
          context.Response.StatusCode = StatusCodes.Status500InternalServerError;
          context.Response.ContentType = "application/json";
          var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
          if (contextFeature != null)
          {
            Log.Error($"Something went wrong in the {contextFeature.Error}");

            await context.Response.WriteAsync(new Error
            {
              StatusCode = context.Response.StatusCode,
              Message = "Internal Server Error. Please try again later"
            }.ToString());
          }
        });
      });
    }

  }
}
