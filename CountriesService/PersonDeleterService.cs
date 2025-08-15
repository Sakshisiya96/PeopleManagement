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
    public class PersonDeleterService : IPersonDeleteService
    {
        private readonly IPersonRepository _personsRespository;
      
        private readonly ILogger<PersonDeleterService> _logger;
        public PersonDeleterService(IPersonRepository personRepostory,ILogger<PersonDeleterService> logger)
        {
            _personsRespository = personRepostory;
            _logger = logger;
           
      
        }
        public async Task<bool> DeletePerson(Guid? personId)
        {
            if (personId == null)
            {
                throw new ArgumentNullException(nameof(personId));
            }
            Person? personResponse = await _personsRespository.GetPersonByPersonId(personId.Value);
            if (personResponse == null)
            {
                return false; ;
            }
            await _personsRespository.DeletePersonByPersonId(personId);
            
            return true;

        }

    }
}
