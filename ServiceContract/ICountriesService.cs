using ServiceContract.DTO;
using Microsoft.AspNetCore.Http;
namespace ServiceContract
{
    public interface ICountriesService
    {
       Task<CountryResponse> AddCountry(CountryAddRequest? countryRequest);
       Task<List<CountryResponse>> GetAllCountries();
       Task<CountryResponse?> GetCountryByCOuntryId(Guid? CountryId);
        /// <summary>
        /// Add a country object to the list od countries
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        Task<int> UploadCountriesFromExcel(IFormFile formFile);
    }
}
