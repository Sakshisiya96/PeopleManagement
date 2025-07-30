using CountriesService;
using Entities;
using Entity;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using NuGet.Frameworks;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using RepositoryContract;
using Moq;
using System.Linq.Expressions;

namespace CrudTest
{
    public class PersonServiceTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;//parent interface of AutoFixture, which is used to create test data
        //It Represnt the mocked object that was created by Mock<T>
        private readonly IPersonRepository _personReposiroty;
        //Used to mock the methods of IPersonRepository
        private readonly Mock<IPersonRepository> _personRepositoryMock;
        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personRepositoryMock = new Mock<IPersonRepository>();
            _personReposiroty = _personRepositoryMock.Object;

            var counrtiesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);//default dbcontext option create
            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, counrtiesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);
            _countriesService = new CountriesServiceM(null);
            _personService = new PersonService(_personReposiroty);
            _testOutputHelper = testOutputHelper;
        }
        [Fact]
        public async Task AddPerson_NullPersons_ToBeArgumentNullException()
        {
            PersonAddRequest? personAddRequest = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personService.AddPerson(personAddRequest);

            });
        }
        [Fact]
        public async Task AddPerson_NullPersonName_ToBeArgumentException()
        {
            PersonAddRequest? personaddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, null as string).Create();
            Person person = personaddRequest.ToPerson();

            //when PersonRepository AddPerson is called it has to be return same person object
            _personRepositoryMock.Setup(temp => temp.AddPersons(It.IsAny<Person>())).ReturnsAsync(person);
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personService.AddPerson(personaddRequest);
            });
        }
        //when insert the person into person list and it should return an obect od Personresponse, which include
        //with newly genertaed person id
        [Fact]
        public async Task AddPerson_ValidPersonDetail_ToBeSuccess()
        {
            // PersonAddRequest personaddRequest = _fixture.Create<PersonAddRequest>();
            PersonAddRequest personaddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "sakshi@gmail.com").Create();

            Person person = personaddRequest.ToPerson();
            PersonResponse personResponseexpected = person.ToPersonResponse();
            _personRepositoryMock.Setup(temp => temp.AddPersons(It.IsAny<Person>())).ReturnsAsync(person);

            PersonResponse personresponse = await _personService.AddPerson(personaddRequest);

            personResponseexpected.PersonId = personresponse.PersonId;

            personresponse.PersonId.Should().NotBe(Guid.Empty);

            personresponse.Should().Be(personResponseexpected);
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
            Person person = _fixture.Build<Person>()
                .With(p => p.Email, "sakshi@gmail.com")
                .With(temp => temp.Country, null as Country).
                Create();


            PersonResponse personresponse_expectedResponse = person.ToPersonResponse();
            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);

            //Assert
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonId(person.PersonId);
            person_response_from_get.Should().Be(personresponse_expectedResponse);

        }
        [Fact]
        public async Task GetAllPerson_EmptyList()
        {
            var person = new List<Person>();
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(person);
            List<PersonResponse> list = await _personService.GetAllPersons();
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetAllPerson_WithFewPerson_ToBeSuccessfully()
        {

            List<Person> person = new List<Person>(){
                _fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),
                 _fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),_fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create()


            };
            List<PersonResponse> person_response_list_expected = person.Select(temp => temp.ToPersonResponse()).ToList();

            foreach (PersonResponse person_request in person_response_list_expected)
            {
                person_request.ToString();
            }
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(person);
            //Act
            List<PersonResponse> person_list_from_get = await _personService.GetAllPersons();

            //Assert
            foreach (PersonResponse person_response_list_from_get in person_list_from_get)
            {
                _testOutputHelper.WriteLine(
                person_response_list_from_get.ToString());
            }
            person_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }
        #region GetFilterdPerson
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            List<Person> person = new List<Person>(){
                _fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),
                 _fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),_fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create()
            };
            List<PersonResponse> person_response_list_expected = person.Select(temp => temp.ToPersonResponse()).ToList();
             foreach (PersonResponse person_request in person_response_list_expected)
            {
               _testOutputHelper.WriteLine(person_request.ToString());
            }
            _personRepositoryMock.Setup(temp => temp.GetFilteredPerson(It.IsAny<Expression<Func<Person, bool>>>()))
               .ReturnsAsync(person);
            //Act
            List<PersonResponse> person_list_from_get = await _personService.GetFilteredPerson(nameof(Person.PersonName), "");

            //Assert
            foreach (PersonResponse person_response_list in person_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_list.ToString());
            }
            person_list_from_get.Should().BeEquivalentTo(person_response_list_expected);

        }
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            List<Person> person = new List<Person>(){
                _fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),
                 _fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),_fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create()
            };
            List<PersonResponse> person_response_list_expected = person.Select(temp => temp.ToPersonResponse()).ToList();
            foreach (PersonResponse person_request in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_request.ToString());
            }
            _personRepositoryMock.Setup(temp => temp.GetFilteredPerson(It.IsAny<Expression<Func<Person, bool>>>()))
               .ReturnsAsync(person);
            //Act
            List<PersonResponse> person_list_from_get = await _personService.GetFilteredPerson(nameof(Person.PersonName), "Sa");

            //Assert
            foreach (PersonResponse person_response_list in person_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_list.ToString());
            }
            person_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }

        #endregion
        #region GetSorted Persons
        [Fact]
        public async Task GetSortedPersons_TOBESuccessfully()
        {
            List<Person> person = new List<Person>(){
                _fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),
                 _fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create(),_fixture.Build<Person>()
                .With(temp => temp.Email, "abc@gmial.com")
                .With(temp=>temp.Country,null as Country)
                .Create()
            };
            List<PersonResponse> person_response_list_expected = person.Select(temp => temp.ToPersonResponse()).ToList();
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(person);
            foreach(PersonResponse person_reponse_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_reponse_from_add.ToString());
            }
            List<PersonResponse> allpersons = await _personService.GetAllPersons();
            //Act
            List<PersonResponse> person_list_from_sort = await _personService.GetSortedPersons(allpersons, nameof(Person.PersonName), SeacrhOrderOption.DESC);

            foreach (PersonResponse person_request in person_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_request.ToString());
            }
                person_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }
        #endregion

        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_null()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
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
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personService.UpdatePerson(personUpdateRequest);
            });

        }
        [Fact]
        public async Task UpdatePerson_PersonNameInvalid()
        {
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            //generating the countryid
            CountryResponse personResponse = await _countriesService.AddCountry(countryAddRequest);
            //addign inside the personaddrequest

            PersonAddRequest personreq = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sakshi@gmai.com")
                .Create();
            PersonResponse pr = await _personService.AddPerson(personreq);
            //in person response we have person request
            PersonUpdateRequest pur = pr.ToPersonUpdateRequest();
            pur.PersonName = null;
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personService.UpdatePerson(pur);
            });
        }
        //add new person and update the same
        [Fact]
        public async Task UpdatePerson_ValidPersonRequest()
        {
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            //generating the countryid
            CountryResponse personResponse = await _countriesService.AddCountry(countryAddRequest);
            //addign inside the personaddrequest

            PersonAddRequest personreq = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mish")
                .With(temp => temp.Email, "mis@gmail.com")
                .With(temp => temp.CountryId, personResponse.CountryId)
                .Create();
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
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryResponse country = await _countriesService.AddCountry(countryAddRequest);
            PersonAddRequest personreq = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mish")
                .With(temp => temp.Email, "mis@gmail.com")
                .With(temp => temp.CountryId, country.CountryId)
                .Create();
            PersonResponse personresponse = await _personService.AddPerson(personreq);
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
