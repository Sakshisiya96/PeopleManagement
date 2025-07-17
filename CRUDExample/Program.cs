using ServiceContract;
using CountriesService;
using Microsoft.EntityFrameworkCore;
using Entity;
using System;
using Entities;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

//Add services inside the Ioc Containers
builder.Services.AddScoped<ICountriesService, CountriesServiceM>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddDbContext<PersonsDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));//scoped service
});
var app = builder.Build();
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.Run();


