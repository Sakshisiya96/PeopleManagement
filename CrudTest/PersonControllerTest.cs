using AutoFixture;
using Castle.Core.Logging;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
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
        private readonly IPersonGetterService _personGetterService;
        private readonly IPersonAdderService _personAdderService;
        private readonly IPersonDeleteService _personDeleterService;
        private readonly IPersonSortedService _personSortedService;
        private readonly IPersonUpdaterService _personUpdaterService;
        private readonly ICountriesService _countriesService;   
        private readonly Mock<IPersonGetterService> _personGetterServiceMock;
        private readonly Mock<IPersonAdderService> _personAdderServiceMock;
        private readonly Mock<IPersonDeleteService> _personDeleterServiceMock;
        private readonly Mock<IPersonSortedService> _personSortedServiceMock;
        private readonly Mock<IPersonUpdaterService> _personUpdaterServiceMock;
        private readonly Mock<ICountriesService> _countryServiceMock;
        private readonly ILogger<PersonsController> _logger;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;
        private readonly Fixture _fixture;
        public PersonControllerTest()
        {
            _fixture=new Fixture();
            _countryServiceMock = new Mock<ICountriesService>();
            _personGetterServiceMock= new Mock<IPersonGetterService>();
            _personAdderServiceMock= new Mock<IPersonAdderService>();
            _personDeleterServiceMock= new Mock<IPersonDeleteService>();
            _personSortedServiceMock= new Mock<IPersonSortedService>();
            _personUpdaterServiceMock= new Mock<IPersonUpdaterService>();
            _loggerMock = new Mock<ILogger<PersonsController>>();
            _countriesService = _countryServiceMock.Object;
            _personGetterService = _personGetterServiceMock.Object;
            _personAdderService = _personAdderServiceMock.Object;
            _personDeleterService = _personDeleterServiceMock.Object;
            _personSortedService = _personSortedServiceMock.Object;
            _personUpdaterService = _personUpdaterServiceMock.Object;
            _logger = _loggerMock.Object;
        }
        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            List<PersonResponse> person_Response_list= _fixture.Create<List<PersonResponse>>();
            // Fixing the syntax errors by adding missing commas and correcting variable names
            PersonsController personController = new PersonsController(
                _personGetterService,
                _personAdderService,
                _personDeleterService,
                _personSortedService, // Corrected variable name
                _personUpdaterService,
                _countriesService,
                _logger
            );
            _personGetterServiceMock.Setup(temp => temp.GetFilteredPerson(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(person_Response_list);
            _personSortedServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SeacrhOrderOption>())).ReturnsAsync(person_Response_list);
            IActionResult result=await personController.Index(_fixture.Create<string>(),_fixture.Create<string>(),_fixture.Create<string>(),_fixture.Create<SeacrhOrderOption>());

            Assert.IsType<ViewResult>(result);

        }
        #endregion
    }
}
