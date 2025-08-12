using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using ServiceContract.DTO;
using ServiceContract.Enums;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonsListActionFilters : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilters> _logger;
        public PersonsListActionFilters(ILogger<PersonsListActionFilters> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.OnActionExecuted method",nameof(PersonsListActionFilters),nameof(OnActionExecuted));
            PersonsController personsController = (PersonsController)context.Controller;
            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];
            if (parameters != null)
            {
                if (parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);
                }

                if (parameters.ContainsKey("searchString"))
                {
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
                }

                if (parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
                }
                else
                {
                    personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
                }

                if (parameters.ContainsKey("sortOrder"))
                {
                    personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
                }
                else
                {
                    personsController.ViewData["CurrentSortOrder"] = nameof(SeacrhOrderOption.ASC);
                }
            }
            personsController.ViewBag.SearchFields = new Dictionary<string, string>()
            {
                //Searching
                { nameof(PersonResponse.PersonName),"Person Name" },
                { nameof(PersonResponse.Email),"Email" },
                { nameof(PersonResponse.Address),"Address" },
                { nameof(PersonResponse.DateOfBirth),"Date of Birth" },
                { nameof(PersonResponse.Age),"Age" },
                { nameof(PersonResponse.Gender),"Gender" },
                { nameof(PersonResponse.Country),"Country" },
                { nameof(PersonResponse.RecieveNewsLetters),"Recieve News Letter" },

            };
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilters), nameof(OnActionExecuted));
            context.HttpContext.Items["arguments"] = context.ActionArguments;

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                //validate the searchBy parameter value
                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>() {
                  nameof(PersonResponse.PersonName),
                  nameof(PersonResponse.Email),
                  nameof(PersonResponse.DateOfBirth),
                  nameof(PersonResponse.Gender),
                  nameof(PersonResponse.Address)
                };
                    //reset the searchBy paramer value
                    if (searchByOptions.Any(temp => temp == searchBy) == false)
                    {
                        _logger.LogInformation("searchBy actual value {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("searchBy updated value {searchBy}", context.ActionArguments["searchBy"]);
                    }
                }
            }

        }
    }
}
