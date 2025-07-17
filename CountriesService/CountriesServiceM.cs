using System.Runtime.CompilerServices;
using Entities;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContract;
using ServiceContract.DTO;

namespace CountriesService
{
    public class CountriesServiceM : ICountriesService
    {
        //create object of DBContext class
        private readonly PersonsDbContext _db;
        public CountriesServiceM(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;
          
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

            if(await _db.Countries.CountAsync(temp=>temp.CountryName == countryRequest.CountryName) > 0)
            {
                throw new ArgumentNullException(nameof(_db));    
            }
            //convert dto to domain model
            Country request = countryRequest.ToCounty();

            request.CountryID = Guid.NewGuid();

 
            //Add country object into _countries
             _db.Countries.Add(request);
            await _db.SaveChangesAsync();

            return request.ToCountryResponse();

        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(temp => temp.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCOuntryId(Guid? CountryId)
        {
            if (CountryId == null)
            {
                return null;
            }
            Country? response= await _db.Countries.FirstOrDefaultAsync(country => country.CountryID == CountryId);
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
                        if (_db.Countries.Where(temp => temp.CountryName == countryName).Count() == 0)
                        {
                            Country countryAddrequest = new Country()
                            {
                                CountryName = countryName
                            };
                            _db.Countries.Add(countryAddrequest);
                            await _db.SaveChangesAsync();
                            countryInserted++;
                        }
                    }
                }
            }
            return countryInserted;

        }
    }
}
