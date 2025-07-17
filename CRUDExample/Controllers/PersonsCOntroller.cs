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
    public class PersonsController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        public PersonsController(IPersonService personService, ICountriesService countriesService)
        {
            _personService = personService;
            _countriesService = countriesService;
        }
        [Route("index")]
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SeacrhOrderOption sortOrder = SeacrhOrderOption.ASC)
        {
            ViewBag.SearchFields = new Dictionary<string, string>()
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

            List<PersonResponse> persons = await _personService.GetFilteredPerson(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;
            //sorting 
            List<PersonResponse> sortedPersons = await _personService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View(sortedPersons);
        }
        [Route("create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp=> new SelectListItem() { Text= temp.CountryName,
            Value=temp.CountryId.ToString()});
            //new SelectListItem() { Text="Sakshi",Value="1"}
            return View();
        }
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
            PersonResponse peronseResponse = await _personService.AddPerson(personAddRequest);
            //navigate to Index() action method to makes another get request to persons/index
            return RedirectToAction("Index", "Persons");
        }
        [HttpGet]
        [Route("Edit/personId")]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse personsResponse=await _personService.GetPersonByPersonId(personId);
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
        public async Task<IActionResult> Edit(PersonUpdateRequest personReq,Guid personId)
        {
            PersonResponse? personsResponse = await _personService.GetPersonByPersonId(personReq.PersonId);
            if (personsResponse == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                PersonResponse updatedPerson = await _personService.UpdatePerson(personReq);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personsResponse.ToPersonUpdateRequest());
            }
        }
        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPdf()
        {
            List<PersonResponse> personResponse=await _personService.GetAllPersons();
            return new ViewAsPdf("PersonsPDF", personResponse, ViewData)
            {

                PageMargins=new Rotativa.AspNetCore.Options.Margins() { Top=20,Right=20,Bottom=20,Left=20},
                PageOrientation=Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }
        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
           MemoryStream per= await _personService.GetPersonsCSV();
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
