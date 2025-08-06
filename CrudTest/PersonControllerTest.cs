using AutoFixture;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Moq;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudTest
{
    public class PersonControllerTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;   
        private readonly Mock<IPersonService> _personServiceMock;
        private readonly Mock<ICountriesService> _countryServiceMock;
        private readonly Fixture _fixture;
        public PersonControllerTest()
        {
            _fixture=new Fixture();
            _countryServiceMock = new Mock<ICountriesService>();
            _personServiceMock= new Mock<IPersonService>();
            _countriesService = _countryServiceMock.Object;
            _personService = _personServiceMock.Object;
        }
        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            List<PersonResponse> person_Response_list= _fixture.Create<List<PersonResponse>>();
            PersonsController personController = new PersonsController(_personService, _countriesService);
            _personServiceMock.Setup(temp => temp.GetFilteredPerson(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(person_Response_list);
            _personServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SeacrhOrderOption>())).ReturnsAsync(person_Response_list);
            IActionResult result=await personController.Index(_fixture.Create<string>(),_fixture.Create<string>(),_fixture.Create<string>(),_fixture.Create<SeacrhOrderOption>());

            Assert.IsType<ViewResult>(result);

        }
        #endregion
    }
}
