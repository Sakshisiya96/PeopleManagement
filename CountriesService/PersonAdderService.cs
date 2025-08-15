using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Entity;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContract;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;
using ServiceContract.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountriesService
{
    public class PersonAdderService : IPersonAdderService
    {
        private readonly IPersonRepository _personsRespository;
      
        private readonly ILogger<PersonAdderService> _logger;
        public PersonAdderService(IPersonRepository personRepostory,ILogger<PersonAdderService> logger)
        {
            _personsRespository = personRepostory;
            _logger = logger;
           
      
        }
        public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
        {
           
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            //if (string.IsNullOrEmpty(request.PersonName))
            //{
            //    throw new ArgumentNullException("Person name cannot be blank");
            //}
            //Model validation
            ValidationHelper.ModelValidation(request);
            //convert dto to domain model
            Person person = request.ToPerson();
            person.PersonId = Guid.NewGuid();
             await _personsRespository.AddPersons(person);
           
            // _db.sp_InsertPersons(person);
            //convert Person object into PersonResponse type
            //return ConvertPersonToPersonResponse(person);
            return person.ToPersonResponse();
        }
    }
}
