using HotelListing.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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

  }
}
