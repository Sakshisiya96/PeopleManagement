using CountriesService;
using Entities;
using Entity;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Repository;
using RepositoryContract;
using ServiceContract;
using System;
using Serilog;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
    // or tailor to Request, Response, Headers, etc.
});
builder.Host.UseSerilog((HostBuilderContext context,IServiceProvider services,LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration).//reading the cofiguration from appsetting.json
    ReadFrom.Services(services);//read the current apps services and make them avaialble to serilog
});
builder.Services.AddHttpLogging(options => options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties
| Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties);
builder.Services.AddControllersWithViews();

//Add services inside the Ioc Containers
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<ICountriesService, CountriesServiceM>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));//scoped service
});
var app = builder.Build();
app.UseHttpLogging();
//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("debug-message");
//app.Logger.LogWarning("debug-message");
//app.Logger.LogError("debug-message");
//app.Logger.LogCritical("debug-message");
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.Run();


