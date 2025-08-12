using CountriesService;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContract;
using ServiceContract.DTO;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonsCreateAndExistPostActioFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countryService;
        public PersonsCreateAndExistPostActioFilter(ICountriesService countryService)
        {
            _countryService = countryService;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context.Controller is PersonsController personController)
            {
                if (!personController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countryService.GetAllCountries();
                    personController.ViewBag.Countries = countries;
                    personController.ViewBag.Errors = personController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    var personAddReques = context.ActionArguments["personRequest"];
                    context.Result= personController.View(personAddReques);
                }
                else
                {
                    await next();//invoke the subsequent filter or action method
                }
            }
            else
            {
                await next();
            }
          
           
        }
    }
}
