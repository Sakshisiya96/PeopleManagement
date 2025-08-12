using Azure.Core;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.Authorization;
using CRUDExample.Filters.ExceptionFilter;
using CRUDExample.Filters.Resources;
using CRUDExample.Filters.ResultFilter;
using CRUDExample.Filters.ResultFolder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;

namespace CRUDExample.Controllers
{
    // [Route("persons")]
    [Route("[controller]")]
    [TypeFilter(typeof(ResponseActiomFilter), Arguments = new object[] { "X-Key-From-Controller", "My-Value-From-Controller" ,3},Order=3)]
    [TypeFilter(typeof(HandleExceptionFilter))]
    public class PersonsController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;
        public PersonsController(IPersonService personService, ICountriesService countriesService, ILogger<PersonsController> logger)
        {
            _personService = personService;
            _countriesService = countriesService;
            _logger = logger;
        }
        [Route("index")]
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilters),Order =4)]
        [TypeFilter(typeof(ResponseActiomFilter), Arguments = new object[] { "X-Custom-Key-From-Action", "Custom-Value-From-Action" ,1},Order =1)]
        [TypeFilter(typeof(PersonsListResultFilterv))]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SeacrhOrderOption sortOrder = SeacrhOrderOption.ASC)
        {
            _logger.LogInformation("Index Action method of PersonsCOntroller");
            _logger.LogDebug($"searchBy:{searchBy},searchString:{searchString},sortBy:{sortBy},sortOrder:{sortOrder}");

            List<PersonResponse> persons = await _personService.GetFilteredPerson(searchBy, searchString);
            //sorting 
            List<PersonResponse> sortedPersons = await _personService.GetSortedPersons(persons, sortBy, sortOrder);
            return View(sortedPersons);
        }
        [Route("create")]
        [HttpGet]
        [TypeFilter(typeof(ResponseActiomFilter),Arguments =new object[] {"my-Key","my-Value",4})]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryId.ToString()
            });
            //new SelectListItem() { Text="Sakshi",Value="1"}
            return View();
        }
        [Route("create")]
        [HttpPost]
        [TypeFilter(typeof(PersonsCreateAndExistPostActioFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFiltercs))]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            PersonResponse peronseResponse = await _personService.AddPerson(personRequest);
            //navigate to Index() action method to makes another get request to persons/index
            return RedirectToAction("Index", "Persons");
        }
        [HttpGet]
        [Route("Edit/personId")]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse personsResponse = await _personService.GetPersonByPersonId(personId);
            if (personsResponse == null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest personUpdateReq = personsResponse.ToPersonUpdateRequest();
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryId.ToString()
            });
            return View(personUpdateReq);
        }
        [HttpPost]
        [Route("Edit/personId")]
        [TypeFilter(typeof(PersonsCreateAndExistPostActioFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest, Guid personId)
        {
            PersonResponse? personsResponse = await _personService.GetPersonByPersonId(personRequest.PersonId);
            if (personsResponse == null)
            {
                return RedirectToAction("Index");
            }
                PersonResponse updatedPerson = await _personService.UpdatePerson(personRequest);
                return RedirectToAction("Index");
        }
        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPdf()
        {
            List<PersonResponse> personResponse = await _personService.GetAllPersons();
            return new ViewAsPdf("PersonsPDF", personResponse, ViewData)
            {

                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }
        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream per = await _personService.GetPersonsCSV();
            return File(per, "application/octet-stream", "persons.csv");
        }
        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream per = await _personService.GetPersonExcel();
            return File(per, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
        [HttpGet]
        [Route("Delete/personId")]
        public async Task<IActionResult> Delete(Guid? personId)
        {
            PersonResponse? personsResponse = await _personService.GetPersonByPersonId(personId);
            if (personsResponse == null)
            {
                return RedirectToAction("Index");
            }
            return View(personsResponse);
        }
        [HttpPost]
        [Route("Delete/personId")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personReq)
        {
            PersonResponse? personsResponse = await _personService.GetPersonByPersonId(personReq.PersonId);
            if (personsResponse == null)
            {
                return RedirectToAction("Index");
            }

            _personService.DeletePerson(personReq.PersonId);
            return RedirectToAction("Index");

        }
    }
}
