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
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personsRespository;
      
        private readonly ILogger<PersonService> _logger;
        public PersonService(IPersonRepository personRepostory,ILogger<PersonService> logger)
        {
            _personsRespository = personRepostory;
            _logger = logger;
           
      
        }
        private async Task<PersonResponse> ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personR = person.ToPersonResponse();
            personR.Country = person.Country?.CountryName;
            return personR;
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

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of Persons Service");
            var person = await _personsRespository.GetAllPersons();
            return  person //select * from persons
                .Select(temp => temp.ToPersonResponse()).ToList();//list of person return

            //return _db.sp_GetAllPersons()
            // .Select(temp => ToPersonResponses(temp)).ToList();//list of person return
        }

        public async Task<PersonResponse> GetPersonByPersonId(Guid? personId)
        {
            if (personId == null)
            {
                return null;
            }
            Person? personres = await _personsRespository.GetPersonByPersonId(personId.Value);
            if (personres == null)
            {
                return null;
            }
            return personres.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPerson(string searchBy, string? seacrhString)
        {
            _logger.LogInformation("GetFilteredPerson of PersonService");
            List<Person> persons = searchBy switch {
                nameof(PersonResponse.PersonName) =>
                await _personsRespository.GetFilteredPerson(x =>
                x.PersonName.Contains(seacrhString)),


                nameof(PersonResponse.Email) =>
                await _personsRespository.GetFilteredPerson(x =>
                x.Email.Contains(seacrhString)),

                nameof(PersonResponse.DateOfBirth) =>
                await _personsRespository.GetFilteredPerson(x =>
                x.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(seacrhString)),

                nameof(PersonResponse.Gender) =>
                await _personsRespository.GetFilteredPerson(x =>
                x.Gender.Contains(seacrhString)),

                nameof(PersonResponse.Address) =>
                await _personsRespository.GetFilteredPerson(x =>
                x.Address.Contains(seacrhString)),

                nameof(PersonResponse.CountryId) =>
                await _personsRespository.GetFilteredPerson(x =>
                x.Country.CountryName.Contains(seacrhString)),

                   _ => await _personsRespository.GetAllPersons()


            };
            return persons.Select(temp=>temp.ToPersonResponse()).ToList();
        }
        /// <summary>
        /// Return sorted list of person
        /// </summary>
        /// <param name="allPersons"></param>
        /// <param name="sortBy"></param>
        /// <param name="SortOrder"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
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

        public async Task<MemoryStream> GetPersonsCSV()
        {
             MemoryStream memoryStream=new MemoryStream();
            StreamWriter streamWriter=new StreamWriter(memoryStream);
            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvwriter = new CsvWriter(streamWriter, csvConfiguration);
            csvwriter.WriteField(nameof(PersonResponse.PersonName));//Person Headers...
            csvwriter.WriteField(nameof(PersonResponse.Email));//Person Headers...
            csvwriter.WriteField(nameof(PersonResponse.DateOfBirth));//Person Headers...
            csvwriter.WriteField(nameof(PersonResponse.Age));//Person Headers...
        //    csvwriter.WriteField(nameof(PersonResponse.Gender));//Person Headers...
            csvwriter.WriteField(nameof(PersonResponse.Country));//Person Headers...
            csvwriter.WriteField(nameof(PersonResponse.Address));//Person Headers...
            csvwriter.WriteField(nameof(PersonResponse.RecieveNewsLetters));//Person Headers...
            csvwriter.NextRecord();
            List<PersonResponse> persons = await GetAllPersons();
            foreach(PersonResponse person in persons)
            {
                csvwriter.WriteField(person.PersonName);
                csvwriter.WriteField(person.Email);
                if (person.DateOfBirth.HasValue)
                    csvwriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                else
                    csvwriter.WriteField("");
                    csvwriter.WriteField(person.Age);
           //     csvwriter.WriteField(person.Gender);
                csvwriter.WriteField(person.Country);
                csvwriter.WriteField(person.Address);
                csvwriter.WriteField(person.RecieveNewsLetters);
                csvwriter.NextRecord();
                csvwriter.Flush();
            }
        
            memoryStream.Position = 0;
            return memoryStream;


        }

        public async Task<MemoryStream> GetPersonExcel()
        {
            MemoryStream memoryStream = new MemoryStream();//any type of file data csv,excel,image files
            using (ExcelPackage package = new ExcelPackage(memoryStream))//create a worksheet or workbook we use ExcelPackage
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add("PersonsSheet");
                workSheet.Cells["A1"].Value = "Person Name";
                workSheet.Cells["B1"].Value = "Email";
                workSheet.Cells["C1"].Value = "Date of Birth";
                workSheet.Cells["D1"].Value = "Age";
                workSheet.Cells["E1"].Value = "Gender";
                workSheet.Cells["F1"].Value = "Country";
                workSheet.Cells["G1"].Value = "Address";
                workSheet.Cells["H1"].Value = "Recieve New Letter";

                using(ExcelRange headercelss = workSheet.Cells["A1:H1"])
                {
                    headercelss.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headercelss.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headercelss.Style.Font.Bold = true;
                }
                int row = 2;
                List<PersonResponse> response = await GetAllPersons();
                foreach(PersonResponse responseItem in response)
                {
                    workSheet.Cells[row, 1].Value = responseItem.PersonName;
                    workSheet.Cells[row, 2].Value = responseItem.Email;
                    if (responseItem.DateOfBirth.HasValue) workSheet.Cells[row, 3].Value = responseItem.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[row, 4].Value = responseItem.Age;
                    workSheet.Cells[row, 5].Value = responseItem.Gender;
                    workSheet.Cells[row, 6].Value = responseItem.Country;
                    workSheet.Cells[row, 7].Value = responseItem.Address;
                    workSheet.Cells[row, 8].Value = responseItem.RecieveNewsLetters;
                    row++;
                }
                workSheet.Cells[$"A1:H{row}"].AutoFitColumns();
                await package.SaveAsync();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
