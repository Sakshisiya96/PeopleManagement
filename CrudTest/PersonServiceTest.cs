using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CountriesService;
using Entities;
using Entity;
using Microsoft.EntityFrameworkCore;
using NuGet.Frameworks;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;
using Xunit.Abstractions;

namespace CrudTest
{
    public class PersonServiceTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _countriesService = new CountriesServiceM(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
            _personService = new PersonService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countriesService);
            _testOutputHelper = testOutputHelper;
        }
        [Fact]
        public async Task AddPerson_Empty()
        {
            PersonAddRequest? personAddRequest = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                await _personService.AddPerson(personAddRequest);

            });
        }
        [Fact]
        public async Task AddPerson_NullPersonName()
        {
            PersonAddRequest? personaddRequest = new PersonAddRequest()
            {
                PersonName = null
            };
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                await _personService.AddPerson(personaddRequest);
            });
        }
        //when insert the person into person list and it should return an obect od Personresponse, which include
        //with newly genertaed person id
        [Fact]
        public async Task AddPerson_ValidPersonDetail()
        {
            PersonAddRequest personaddRequest = new PersonAddRequest()
            {
                PersonName = "Ram",
                Address = "Gorakhpur",
                Email = "sak@gmail.com",
                CountryId = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-10"),
                RecieveNewsLetters = true
            };
            PersonResponse personresponse = await _personService.AddPerson(personaddRequest);
            List<PersonResponse> responseList = await _personService.GetAllPersons();
            Assert.Contains(personresponse, responseList);
            Assert.True(personresponse.PersonId != Guid.Empty);
        }
        [Fact]
        public async Task GetPersonByPersonId_IdNull()
        {
            Guid? personId = null;
            PersonResponse? person = await _personService.GetPersonByPersonId(personId);
            Assert.Null(person);
        }
        //if we supply valida person id, it should return the valida person detils as personResponse object
        [Fact]
        public async Task GetPersonByPersonId_NameByID()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Japan"
            };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
            PersonAddRequest personaddRequest = new PersonAddRequest()
            {
                PersonName = "sakshi",
                Email = "sak@gmai.com",
                Address = "Gkp",
                CountryId = countryResponse.CountryId,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = GenderOptions.Male,
                RecieveNewsLetters = true

            };

            PersonResponse personres = await _personService.AddPerson(personaddRequest);

            //Assert
            PersonResponse PersonNameById = await _personService.GetPersonByPersonId(personres.PersonId);
            Assert.Equal(personres, PersonNameById);

        }
        [Fact]
        public async Task GetAllPerson_EmptyList()
        {
            List<PersonResponse> list = await _personService.GetAllPersons();
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetAllPerson_GetPersonlist()
        {
            CountryAddRequest cd1 = new CountryAddRequest()
            {
                CountryName = "US"
            };
            CountryAddRequest cd2 = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse countryresponse1 = await _countriesService.AddCountry(cd1);
            CountryResponse countryresponse2 = await _countriesService.AddCountry(cd2);
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "sak",
                Email = "sak@gmai.com",
                CountryId = countryresponse1.CountryId,
                DateOfBirth = DateTime.Parse("2000-01-10")

            };
            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "sakhi",
                Email = "sakhi@gmai.com",
                CountryId = countryresponse2.CountryId,
                DateOfBirth = DateTime.Parse("2000-07-10")

            };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { personAddRequest, personAddRequest1 };
            foreach (var person_request in person_requests)
            {
                person_response_list_from_add.Add(await _personService.AddPerson(person_request));
            }
            //Act
            List<PersonResponse> person_list_from_get = await _personService.GetAllPersons();

            //Assert
            foreach (PersonResponse person_response_list in person_response_list_from_add)
            {
                Assert.Contains(person_response_list, person_list_from_get);
            }
        }
        #region GetFilterdPerson
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            CountryAddRequest cd1 = new CountryAddRequest()
            {
                CountryName = "US"
            };
            CountryAddRequest cd2 = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse countryresponse1 = await _countriesService.AddCountry(cd1);
            CountryResponse countryresponse2 = await _countriesService.AddCountry(cd2);
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "sak",
                Email = "sak@gmai.com",
                CountryId = countryresponse1.CountryId,
                DateOfBirth = DateTime.Parse("2000-01-10")

            };
            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "sakhi",
                Email = "sakhi@gmai.com",
                CountryId = countryresponse2.CountryId,
                DateOfBirth = DateTime.Parse("2000-07-10")

            };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { personAddRequest, personAddRequest1 };
            foreach (var person_request in person_requests)
            {
                person_response_list_from_add.Add(await _personService.AddPerson(person_request));
            }
            //Act
            List<PersonResponse> person_list_from_get = await _personService.GetFilteredPerson(nameof(Person.PersonName), "");

            //Assert
            foreach (PersonResponse person_response_list in person_response_list_from_add)
            {
                Assert.Contains(person_response_list, person_list_from_get);
            }

        }
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            CountryAddRequest cd1 = new CountryAddRequest()
            {
                CountryName = "US"
            };
            CountryAddRequest cd2 = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse countryresponse1 = await _countriesService.AddCountry(cd1);
            CountryResponse countryresponse2 = await _countriesService.AddCountry(cd2);
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "sak",
                Email = "sak@gmai.com",
                CountryId = countryresponse1.CountryId,
                DateOfBirth = DateTime.Parse("2000-01-10")

            };
            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "sakhi",
                Email = "sakhi@gmai.com",
                CountryId = countryresponse2.CountryId,
                DateOfBirth = DateTime.Parse("2000-07-10")

            };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { personAddRequest, personAddRequest1 };
            foreach (var person_request in person_requests)
            {
                person_response_list_from_add.Add(await _personService.AddPerson(person_request));
            }
            //Act
            List<PersonResponse> person_list_from_get = await _personService.GetFilteredPerson(nameof(Person.PersonName), "sak");

            //Assert
            foreach (PersonResponse person_response_list in person_response_list_from_add)
            {
                if (person_response_list.PersonName != null)
                {
                    if (person_response_list.PersonName.Contains("sak", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_list, person_list_from_get);
                    }
                }
            }

        }

        #endregion
        #region GetSorted Persons
        [Fact]
        public async Task GetSortedPersons()
        {
            CountryAddRequest cd1 = new CountryAddRequest()
            {
                CountryName = "US"
            };
            CountryAddRequest cd2 = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse countryresponse1 = await _countriesService.AddCountry(cd1);
            CountryResponse countryresponse2 = await _countriesService.AddCountry(cd2);
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "sak",
                Email = "sak@gmai.com",
                CountryId = countryresponse1.CountryId,
                DateOfBirth = DateTime.Parse("2000-01-10"),
                Address = "Gkp",
                Gender = GenderOptions.Male,
                RecieveNewsLetters = true,

            };
            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "sakhi",
                Email = "sakhi@gmai.com",
                CountryId = countryresponse2.CountryId,
                DateOfBirth = DateTime.Parse("2000-07-10"),
                Address = "Gkp",
                Gender = GenderOptions.Male,
                RecieveNewsLetters = true,

            };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { personAddRequest, personAddRequest1 };
            foreach (var person_request in person_requests)
            {
                person_response_list_from_add.Add(await _personService.AddPerson(person_request));
            }
            List<PersonResponse> allpersons = await _personService.GetAllPersons();
            //Act
            List<PersonResponse> person_list_from_sort = await _personService.GetSortedPersons(allpersons, nameof(Person.PersonName), SeacrhOrderOption.DESC);
            person_response_list_from_add = person_response_list_from_add.OrderByDescending(x => x.PersonName).ToList();
            //Assert
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], person_list_from_sort[i]);

            }
        }
        #endregion

        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_null()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
               await _personService.UpdatePerson(personUpdateRequest);

            });
        }
        [Fact]
        public async Task UpdatePerson_PersonIdInvalid()
        {
            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
            {
                PersonId = Guid.NewGuid()
            };
            await  Assert.ThrowsAsync<ArgumentException>(async() =>
            {
               await _personService.UpdatePerson(personUpdateRequest);
            });

        }
        [Fact]
        public async Task UpdatePerson_PersonNameInvalid()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "US"
            };
            //generating the countryid
            CountryResponse personResponse = await _countriesService.AddCountry(countryAddRequest);
            //addign inside the personaddrequest

            PersonAddRequest personreq = new PersonAddRequest()
            {
                PersonName = "john",
                CountryId = personResponse.CountryId,
                Email = "john@gmai.com",
                DateOfBirth = DateTime.Parse("2000-02-10"),
                Address = "US",
                Gender = GenderOptions.Male
            };
            PersonResponse pr = await _personService.AddPerson(personreq);
            //in person response we have person request
            PersonUpdateRequest pur = pr.ToPersonUpdateRequest();
            pur.PersonName = null;
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
               await  _personService.UpdatePerson(pur);
            });
        }
        //add new person and update the same
        [Fact]
        public async Task UpdatePerson_ValidPersonRequest()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "US"
            };
            //generating the countryid
            CountryResponse personResponse = await _countriesService.AddCountry(countryAddRequest);
            //addign inside the personaddrequest

            PersonAddRequest personreq = new PersonAddRequest()
            {
                PersonName = "john",
                CountryId = personResponse.CountryId,
                Address = "uk",
                DateOfBirth = DateTime.Parse("2000-01-07"),

                Email = "sak@gmail.com",
                Gender = GenderOptions.Female,
                RecieveNewsLetters = true

            };
            PersonResponse pr = await _personService.AddPerson(personreq);
            //in person response we have person request
            PersonUpdateRequest pur = pr.ToPersonUpdateRequest();
            pur.PersonName = "Mish";
            pur.Email = "mis@gma";

            PersonResponse personresponse = await _personService.UpdatePerson(pur);

            PersonResponse call = await _personService.GetPersonByPersonId(personresponse.PersonId);
            Assert.Equal(call, personresponse);

        }

        #endregion
        #region Delete Region
        [Fact]
        public async Task DeletePerson_ValidPersonId()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse country = await _countriesService.AddCountry(countryAddRequest);
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "tan",
                CountryId = country.CountryId,
                Email = "tan@gmai.com",
                Address = "Japan",
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("2000-10-07"),
                RecieveNewsLetters = true
            };

            PersonResponse personresponse = await _personService.AddPerson(personAddRequest);
            bool is_deleted = await _personService.DeletePerson(personresponse.PersonId);
            Assert.True(is_deleted);

        }
        [Fact]
        public async Task DeletePerson_InValidPersonId()
        {
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());
            Assert.False(isDeleted);

        }
        #endregion
    }
}
