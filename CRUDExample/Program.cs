using ServiceContract;
using CountriesService;
using Microsoft.EntityFrameworkCore;
using Entity;
using System;
using Entities;
using RepositoryContract;
using Repository;
var builder = WebApplication.CreateBuilder(args);

//logging
builder.Host.ConfigureLogging(loggingProvider =>
{
    loggingProvider.ClearProviders();
    loggingProvider.AddConsole();
    loggingProvider.AddDebug();
    loggingProvider.AddEventLog();
});
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


