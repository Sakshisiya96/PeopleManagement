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
    //[TypeFilter(typeof(HandleExceptionFilter))]
    public class PersonsController : Controller
    {
        private readonly IPersonGetterService _personGetterService;
        private readonly IPersonAdderService _personAdderService;
        private readonly IPersonUpdaterService _personUpdaterService;
        private readonly IPersonDeleteService _personDeleteService;
        private readonly IPersonSortedService _personSortedervice;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;
        public PersonsController(IPersonGetterService personService,IPersonAdderService personAddService,IPersonDeleteService personDeleteservice,IPersonSortedService personSortedService,IPersonUpdaterService personUdpaterService, ICountriesService countriesService, ILogger<PersonsController> logger)
        {
            _personGetterService = personService;
            _personAdderService = personAddService;
            _personDeleteService = personDeleteservice;
            _personSortedervice = personSortedService;
            _personUpdaterService = personUdpaterService;
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

            List<PersonResponse> persons = await _personGetterService.GetFilteredPerson(searchBy, searchString);
            //sorting 
            List<PersonResponse> sortedPersons = await _personSortedervice.GetSortedPersons(persons, sortBy, sortOrder);
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
       //[TypeFilter(typeof(FeatureDisabledResourceFiltercs))]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            PersonResponse peronseResponse = await _personAdderService.AddPerson(personRequest);
            //navigate to Index() action method to makes another get request to persons/index
            return RedirectToAction("Index", "Persons");
        }
        [HttpGet]
        [Route("Edit/personId")]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse personsResponse = await _personGetterService.GetPersonByPersonId(personId);
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
            PersonResponse? personsResponse = await _personGetterService.GetPersonByPersonId(personRequest.PersonId);
            if (personsResponse == null)
            {
                return RedirectToAction("Index");
            }
                PersonResponse updatedPerson = await _personUpdaterService.UpdatePerson(personRequest);
                return RedirectToAction("Index");
        }
        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPdf()
        {
            List<PersonResponse> personResponse = await _personGetterService.GetAllPersons();
            return new ViewAsPdf("PersonsPDF", personResponse, ViewData)
            {

                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }
        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream per = await _personGetterService.GetPersonsCSV();
            return File(per, "application/octet-stream", "persons.csv");
        }
        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream per = await _personGetterService.GetPersonExcel();
            return File(per, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
        [HttpGet]
        [Route("Delete/personId")]
        public async Task<IActionResult> Delete(Guid? personId)
        {
            PersonResponse? personsResponse = await _personGetterService.GetPersonByPersonId(personId);
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
            PersonResponse? personsResponse = await _personGetterService.GetPersonByPersonId(personReq.PersonId);
            if (personsResponse == null)
            {
                return RedirectToAction("Index");
            }

            await _personDeleteService.DeletePerson(personReq.PersonId);//without await it will redirect to the same code
            return RedirectToAction("Index");

        }
    }
}
