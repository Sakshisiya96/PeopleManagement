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
    public class PersonUpdaterService : IPersonUpdaterService
    {
        private readonly IPersonRepository _personsRespository;
      
        private readonly ILogger<PersonUpdaterService> _logger;
        public PersonUpdaterService(IPersonRepository personRepostory,ILogger<PersonUpdaterService> logger)
        {
            _personsRespository = personRepostory;
            _logger = logger;
           
      
        }
        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidationHelper.ModelValidation(request);

            //get matching person object to update
            Person? matchingPerson = await _personsRespository.GetPersonByPersonId(request.PersonId);
            if (matchingPerson == null)
            {
                throw new InvalidPersonIdException("Given person id is not exists");
            }
            matchingPerson.PersonName = request.PersonName;
            matchingPerson.Gender = request.Gender.ToString();
            matchingPerson.Address = request.Address;
            matchingPerson.DateOfBirth = request.DateOfBirth;
            matchingPerson.Email = request.Email;
            matchingPerson.CountryId = request.CountryId;
            matchingPerson.RecieveNewsLetters = request.RecieveNewsLetters;
            await _personsRespository.UpdatePerson(matchingPerson);
           
            return matchingPerson.ToPersonResponse();

        }

    }
}
