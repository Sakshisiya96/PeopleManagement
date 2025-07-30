using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CountriesService;
using Entities;
using Microsoft.EntityFrameworkCore;
using NuGet.Frameworks;
using ServiceContract;
using ServiceContract.DTO;
using EntityFrameworkCoreMock;
using Moq;
using Entity;

namespace CrudTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countries;
        public CountriesServiceTest()
        {
            var counrtiesInitialData = new List<Country>() { };
            DbContextMock<ApplicationDbContext> dbContextMock=new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);//default dbcontext option create
            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, counrtiesInitialData);
            _countries = new CountriesServiceM(null);
        }
        #region AddRegion
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            CountryAddRequest? request = null;
            
           await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                await _countries.AddCountry(request);

            });
        }
        [Fact]
        public async Task  AddCountry_CountryNameNull()
        {
            CountryAddRequest request = new CountryAddRequest()
            {
                CountryName = null
            };
           await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
               await _countries.AddCountry(request);
            });
        }
        [Fact]
        public async Task AddCountry_CountryDuplicate()
        {
            CountryAddRequest request1 = new CountryAddRequest()
            {
                CountryName = "Japan"
            };
            CountryAddRequest request2 = new CountryAddRequest()
            {
                CountryName = "Japan"
            };
           await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
               await _countries.AddCountry(request1);
               await _countries.AddCountry(request2);

            });

        }
        //when the supply proper name it shoukd insert the country to the existing list of country to the
        //existing list of countries 
        [Fact]
        public async Task AddCountry_InsertData()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };
           CountryResponse cr= await _countries.AddCountry(request);
            List<CountryResponse> list= await _countries.GetAllCountries();
            Assert.True(cr.CountryId !=Guid.Empty);
            Assert.Contains(cr, list);
        }
        #endregion

        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            List<CountryResponse> actual_country_eresponse = await _countries.GetAllCountries();

            Assert.Empty(actual_country_eresponse);
        }
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
            {
             new CountryAddRequest(){CountryName="USA"},
             new CountryAddRequest(){CountryName="UK"},
            };
            List<CountryResponse> countryResponses = new List<CountryResponse>();

            foreach (CountryAddRequest country in country_request_list)
            {
                countryResponses.Add(await _countries.AddCountry(country));
            }
            List<CountryResponse> actualCountryResponselist=await _countries.GetAllCountries();

            foreach(CountryResponse expect in countryResponses)
            {
                Assert.Contains(expect, actualCountryResponselist);
            }

        }
        [Fact]
        public async Task GetCountryByCountryId_NullCountryId()
        {
            Guid? countryId = null;

            CountryResponse? cr=await _countries.GetCountryByCOuntryId(countryId);
            Assert.Null(cr);

        }
        //if we supply a valid country id , it should return the macthing country details as CountryResponse obj
        [Fact]
        public async Task GetCountryByCountryId_Value()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse? add_country_list=await  _countries.AddCountry(countryAddRequest);
            CountryResponse? get_countryId= await _countries.GetCountryByCOuntryId(add_country_list.CountryId);
            Assert.Equal(add_country_list, get_countryId);
             
        }
    }
}
