using CountriesService;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.StartupExtension;
using Entities;
using Entity;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Repository;
using RepositoryContract;
using Serilog;
using ServiceContract;
using System;
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


builder.Services.ConfigureServices(builder.Configuration);


var app = builder.Build();
app.UseHttpLogging();
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.Run();


