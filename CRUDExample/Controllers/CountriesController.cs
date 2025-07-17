using Microsoft.AspNetCore.Mvc;
using ServiceContract;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesService _countriesService;
        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }
        [Route("UploadFromExcel")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }
        [HttpPost]
        [Route("UploadFromExcel")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length==0)
            {
                ViewBag.ErrorMessage = "Please select an excel sheet";
                return View();
            }
            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Please unsupported file";
                return View();
            }
           int countryCount=await _countriesService.UploadCountriesFromExcel(excelFile);
            ViewBag.Message = $"{countryCount} Countries Uploaded";
            return View();

        }
    }
}
