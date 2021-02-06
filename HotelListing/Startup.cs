using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Repository;
using HotelListing.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace HotelListing
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    //---------------------------------------------------------------------------------------------
    // This method gets called by the runtime. Use it to add services to the container
    //---------------------------------------------------------------------------------------------
    public void ConfigureServices(IServiceCollection services)
    {
      var connectionString = Configuration.GetConnectionString("DefaultConnection");
      services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

      services.AddAuthentication();
      services.ConfigureIdentity();
      services.ConfigureJWT(Configuration);

      services.AddCors(o =>
      {
        o.AddPolicy("AllowAll", builder => builder
          .AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader()
        );
      });

      services.AddAutoMapper(typeof(Startup));

      // AddSingleton : only one instance will exist for the entire duration of the application
      // AddTransient : every time that is needed a new instance will be created
      // AddScoped    : a new instance is created for a period or lifetime of the request
      services.AddTransient<IUnitOfWork, UnitOfWork>();
      services.AddScoped<IAuthManager, AuthManager>();

      // Add NewtonsoftJson to set option that ignores cycle reference between 2 entities
      services.AddControllers().AddNewtonsoftJson(
        op => op.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
      });
    }

    //---------------------------------------------------------------------------------------------
    // This method gets called by the runtime. Use it to configure the HTTP request pipeline
    //---------------------------------------------------------------------------------------------
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      // Moved these 2 lines out of the env.IsDevelopment() block above
      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListing v1"));

      app.UseHttpsRedirection();

      app.UseCors("AllowAll");

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }

  }
}
