using CountriesService;
using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using RepositoryContract;
using ServiceContract;

namespace CRUDExample.StartupExtension
{
    public static class ConfiguredServiecExtension 
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection service,IConfiguration configuration)
        {
            service.AddHttpLogging(options => options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties
| Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties);


            //it adds controllers and views as servicess
            service.AddControllersWithViews(options => {
                //options.Filters.Add<ResponseActiomFilter>(5);
                var logger = service.BuildServiceProvider().GetRequiredService<ILogger<ResponseActiomFilter>>();
                options.Filters.Add(new ResponseActiomFilter(logger, "My-Key-From-Global", "My-Value-FRom-Global", 2));
            });

            //Add services inside the Ioc Containers
            service.AddScoped<ICountryRepository, CountryRepository>();
            service.AddScoped<IPersonRepository, PersonRepository>();
            service.AddScoped<ICountriesService, CountriesServiceM>();
            service.AddScoped<IPersonService, PersonService>();
            service.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));//scoped service
            });
            return service;
        }
    }
}
