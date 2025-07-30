using System.Runtime.CompilerServices;
using Entities;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContract;
using ServiceContract;
using ServiceContract.DTO;

namespace CountriesService
{
    public class CountriesServiceM : ICountriesService
    {
        //create object of DBContext class
        private readonly ICountryRepository _countriesRepository;
        public CountriesServiceM(ICountryRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
          
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryRequest)
        {
            
            if(countryRequest == null)
            {
                throw new ArgumentNullException(nameof(countryRequest));
            }
            if (countryRequest.CountryName == null)
            {
                throw new ArgumentNullException(nameof(countryRequest));
            }
            //Validation country can't be duplicate

            if(await _countriesRepository.GetCountryByCountryName(countryRequest.CountryName) !=null)
            {
                throw new ArgumentNullException(nameof(_countriesRepository));    
            }
            //convert dto to domain model
            Country request = countryRequest.ToCounty();

            request.CountryID = Guid.NewGuid();

 
            //Add country object into _countries
            await  _countriesRepository.AddCountry(request);
           

            return request.ToCountryResponse();

        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries()).Select(temp => temp.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCOuntryId(Guid? CountryId)
        {
            if (CountryId == null)
            {
                return null;
            }
            Country? response= await _countriesRepository.GetCountryByCOuntryId(CountryId.Value);
            if (response == null)
            {
                return null;
            }
            return response.ToCountryResponse();

        }

        public async Task<int> UploadCountriesFromExcel(IFormFile formFile)
        {
           MemoryStream stream = new MemoryStream();//convert formfile to excel
            await formFile.CopyToAsync(stream);
            int countryInserted = 0;
            using (ExcelPackage excel=new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet=excel.Workbook.Worksheets["Sheet1"];
                int rowsCount = worksheet.Dimension.Rows;
               
                for (int row = 2; row <= rowsCount; row++)
                {
                    string? cellValue=Convert.ToString(worksheet.Cells[row, 1].Value);
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string countryName = cellValue;
                        if (await _countriesRepository.GetCountryByCountryName(countryName)==null)
                        {
                            Country countryAddrequest = new Country()
                            {
                                CountryName = countryName
                            };
                           await _countriesRepository.AddCountry(countryAddrequest);                    
                            countryInserted++;
                        }
                    }
                }
            }
            return countryInserted;

        }
    }
}
