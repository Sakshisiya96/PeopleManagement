using AutoFixture;
using Castle.Core.Logging;
using CountriesService;
using Entities;
using Entity;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NuGet.Frameworks;
using Repository;
using RepositoryContract;
using Serilog;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CrudTest
{
    public class PersonServiceTest
    {
        private readonly IPersonGetterService _personsGetterService;
        private readonly IPersonUpdaterService _personsUpdaterService;
        private readonly IPersonDeleteService _personsDeleterService;
        private readonly IPersonSortedService _personsSorterService;
        private readonly IPersonAdderService _personsAdderService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;//parent interface of AutoFixture, which is used to create test data
        //It Represnt the mocked object that was created by Mock<T>
        private readonly IPersonRepository _personReposiroty;
        //Used to mock the methods of IPersonRepository
        private readonly Mock<IPersonRepository> _personRepositoryMock;
        private readonly ILogger<PersonGetterService> _loggerGetter;
        private readonly ILogger<PersonUpdaterService> _loggerUpdater;
        private readonly ILogger<PersonDeleterService> _loggerDeleter;
        private readonly ILogger<PersonSorterService> _loggerSorter;
        private readonly ILogger<PersonAdderService> _loggerAdder;
        private readonly Mock<ILogger<PersonGetterService>> _loggerGetterMock;
        private readonly Mock<ILogger<PersonUpdaterService>> _loggerUpdaterMock;
        private readonly Mock<ILogger<PersonDeleterService>> _loggerDeleterMock;
        private readonly Mock<ILogger<PersonSorterService>> _loggerSorterMock;
        private readonly Mock<ILogger<PersonAdderService>> _loggerAdderMock;

        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personRepositoryMock = new Mock<IPersonRepository>();
            _personReposiroty = _personRepositoryMock.Object;

            var diagnosticContextMock = new Mock<IDiagnosticContext>();
            _loggerGetterMock = new Mock<ILogger<PersonGetterService>>();
            _loggerUpdaterMock = new Mock<ILogger<PersonUpdaterService>>();
            _loggerDeleterMock = new Mock<ILogger<PersonDeleterService>>();
            _loggerSorterMock = new Mock<ILogger<PersonSorterService>>();
            _loggerAdderMock = new Mock<ILogger<PersonAdderService>>();

            _personsGetterService = new PersonGetterService(_personReposiroty, _loggerGetterMock.Object);

            _personsAdderService = new PersonAdderService(_personReposiroty, _loggerAdderMock.Object);

            _personsDeleterService = new PersonDeleterService(_personReposiroty, _loggerDeleterMock.Object);

            _personsSorterService = new PersonSorterService(_personReposiroty, _loggerSorterMock.Object);

            _personsUpdaterService = new PersonUpdaterService(_personReposiroty, _loggerUpdaterMock.Object);
        }
        [Fact]
        public async Task AddPerson_NullPersons_ToBeArgumentNullException()
        {
            PersonAddRequest? personAddRequest = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);

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
                await _personsAdderService.AddPerson(personaddRequest);
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

            PersonResponse personresponse = await _personsAdderService.AddPerson(personaddRequest);

            personResponseexpected.PersonId = personresponse.PersonId;

            personresponse.PersonId.Should().NotBe(Guid.Empty);

            personresponse.Should().Be(personResponseexpected);
        }
        [Fact]
        public async Task GetPersonByPersonId_IdNull()
        {
            Guid? personId = null;
            PersonResponse? person = await _personsGetterService.GetPersonByPersonId(personId);
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
            PersonResponse? person_response_from_get = await _personsGetterService.GetPersonByPersonId(person.PersonId);
            person_response_from_get.Should().Be(personresponse_expectedResponse);

        }
        [Fact]
        public async Task GetAllPerson_EmptyList()
        {
            var person = new List<Person>();
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(person);
            List<PersonResponse> list = await _personsGetterService.GetAllPersons();
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
            List<PersonResponse> person_list_from_get = await _personsGetterService.GetAllPersons();

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
            List<PersonResponse> person_list_from_get = await _personsGetterService.GetFilteredPerson(nameof(Person.PersonName), "");

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
            List<PersonResponse> person_list_from_get = await _personsGetterService.GetFilteredPerson(nameof(Person.PersonName), "Sa");

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
            List<PersonResponse> allpersons = await _personsGetterService.GetAllPersons();
            //Act
            List<PersonResponse> person_list_from_sort = await _personsSorterService.GetSortedPersons(allpersons, nameof(Person.PersonName), SeacrhOrderOption.DESC);

            foreach (PersonResponse person_request in person_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_request.ToString());
            }
                person_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }
        #endregion

        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_null_ToBeArgumnetNullException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);

            });
        }
        [Fact]
        public async Task UpdatePerson_PersonIdInvalid_ToBeArumentException()
        {
            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
            {
                PersonId = Guid.NewGuid()
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            });

        }
        [Fact]
        public async Task UpdatePerson_PersonNameInvalid_ArgumentException()
        {
           

            Person person = _fixture.Build<Person>()
                .With(temp=>temp.PersonName,null as string)
                .With(temp => temp.Email, "sakshi@gmai.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.Gender,"Male")
                .Create();
            PersonResponse person_response_from_add = person.ToPersonResponse();
            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsUpdaterService.UpdatePerson(person_update_request);
            });

        }
        //add new person and update the same
        [Fact]
        public async Task UpdatePerson_ValidPersonRequest()
        {

            Person person_add_Req = _fixture.Build<Person>()
                .With(temp => temp.PersonName, "Mish")
                .With(temp => temp.Email, "mis@gmail.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();
            PersonResponse person_response_expecated = person_add_Req.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_expecated.ToPersonUpdateRequest();
            _personRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person_add_Req);
            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person_add_Req);


            PersonResponse personresponse_from_update = await _personsUpdaterService.UpdatePerson(person_update_request);

            //Assert
            personresponse_from_update.Should().Be(person_response_expecated);

        }

        #endregion
        #region Delete Region
        [Fact]
        public async Task DeletePerson_ValidPersonId_TOBeSuccess()
        {
            Person personreq = _fixture.Build<Person>()
                .With(temp => temp.PersonName, "Mish")
                .With(temp => temp.Email, "mis@gmail.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();
            
            _personRepositoryMock.Setup(temp=>temp.DeletePersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(true);
            _personRepositoryMock.Setup(temp=>temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(personreq);
            bool is_deleted = await _personsDeleterService.DeletePerson(personreq.PersonId);
            Assert.True(is_deleted);

        }
        [Fact]
        public async Task DeletePerson_InValidPersonId()
        {
            Person personreq = _fixture.Build<Person>()
               .With(temp => temp.PersonName, "Mish")
               .With(temp => temp.Email, "mis@gmail.com")
               .With(temp => temp.Country, null as Country)
               .With(temp => temp.Gender, "Male")
               .Create();
         
            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(true);

            bool isDeleted = await _personsDeleterService.DeletePerson(Guid.NewGuid());
            Assert.False(isDeleted);

        }
        #endregion
    }
}
