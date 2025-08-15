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
    public class PersonSorterService : IPersonSortedService
    {
        private readonly IPersonRepository _personsRespository;
      
        private readonly ILogger<PersonSorterService> _logger;
        public PersonSorterService(IPersonRepository personRepostory, ILogger<PersonSorterService> logger)
        {
            _personsRespository = personRepostory;
            _logger = logger;


        }
        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SeacrhOrderOption SortOrder)
        {
            _logger.LogInformation("GetSortedPerson of Person Service");
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;
            List<PersonResponse> sortedPerson = (sortBy, SortOrder)
            switch
            {
                (nameof(PersonResponse.PersonName), SeacrhOrderOption.ASC)
                => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SeacrhOrderOption.DESC)
               => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SeacrhOrderOption.DESC)
                => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SeacrhOrderOption.ASC)
                => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.DateOfBirth), SeacrhOrderOption.ASC)
                => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),


                (nameof(PersonResponse.DateOfBirth), SeacrhOrderOption.DESC)
                => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SeacrhOrderOption.ASC)
                => allPersons.OrderBy(temp => temp.Age).ToList(),


                (nameof(PersonResponse.Age), SeacrhOrderOption.DESC)
                => allPersons.OrderBy(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Gender), SeacrhOrderOption.ASC)
                => allPersons.OrderBy(temp => temp.Gender).ToList(),


                (nameof(PersonResponse.Gender), SeacrhOrderOption.DESC)
                => allPersons.OrderBy(temp => temp.Gender).ToList(),

                (nameof(PersonResponse.Country), SeacrhOrderOption.ASC)
                => allPersons.OrderBy(temp => temp.Country).ToList(),


                (nameof(PersonResponse.Country), SeacrhOrderOption.DESC)
                => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SeacrhOrderOption.ASC)
                => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.Address), SeacrhOrderOption.DESC)
                => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.RecieveNewsLetters), SeacrhOrderOption.ASC)
                => allPersons.OrderBy(temp => temp.RecieveNewsLetters).ToList(),


                (nameof(PersonResponse.RecieveNewsLetters), SeacrhOrderOption.DESC)
                => allPersons.OrderBy(temp => temp.RecieveNewsLetters).ToList(),

                _ => allPersons

            };
            return sortedPerson;

        }

    }
}
