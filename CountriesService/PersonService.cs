using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Entities;
using Entity;
using Microsoft.EntityFrameworkCore;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;
using ServiceContract.Helper;
using System.Globalization;
using System.IO;
using CsvHelper.Configuration;
using OfficeOpenXml;

namespace CountriesService
{
    public class PersonService : IPersonService
    {
        private readonly PersonsDbContext _db;
        private readonly ICountriesService _countryService;
        public PersonService(PersonsDbContext personDbContext,ICountriesService countryService)
        {
            _db = personDbContext;
            _countryService = countryService;
           
            
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
             _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            // _db.sp_InsertPersons(person);
            //convert Person object into PersonResponse type
            //return ConvertPersonToPersonResponse(person);
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var person = await _db.Persons.Include("Country").ToListAsync();
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
            Person? personres = await _db.Persons.Include("Country").FirstOrDefaultAsync(x => x.PersonId == personId);
            if (personres == null)
            {
                return null;
            }
            return personres.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPerson(string searchBy, string? seacrhString)
        {
            List<PersonResponse> allPerson = await GetAllPersons();
            List<PersonResponse> matchingPerson = allPerson;
            if (string.IsNullOrEmpty(searchBy) && string.IsNullOrEmpty(seacrhString))
                return matchingPerson;
            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPerson = allPerson.Where(x => (!string.IsNullOrEmpty(x.PersonName)
                    ? x.PersonName.Contains(seacrhString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    matchingPerson = allPerson.Where(x => (!string.IsNullOrEmpty(x.Email)
                    ? x.Email.Contains(seacrhString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPerson = allPerson.Where(x => (x.DateOfBirth != null) ?
                    x.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(seacrhString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Address):
                    matchingPerson = allPerson.Where(x => (!string.IsNullOrEmpty(x.Address)
                    ? x.Address.Contains(seacrhString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.CountryId):
                    matchingPerson = allPerson.Where(x => (!string.IsNullOrEmpty(x.Country)
                    ? x.Country.Contains(seacrhString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                default: matchingPerson = allPerson; break;
            }
            return matchingPerson;
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
            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(x => x.PersonId == request.PersonId);
            if (matchingPerson == null)
            {
                throw new ArgumentException("Given person id is not exists");
            }
            matchingPerson.PersonName = request.PersonName;
            matchingPerson.Gender = request.Gender.ToString();
            matchingPerson.Address = request.Address;
            matchingPerson.DateOfBirth = request.DateOfBirth;
            matchingPerson.Email = request.Email;
            await _db.SaveChangesAsync();//Update
            return matchingPerson.ToPersonResponse();

        }

        public async Task<bool> DeletePerson(Guid? personId)
        {
            if (personId == null)
            {
                throw new ArgumentNullException(nameof(personId));
            }
            Person? personResponse = await _db.Persons.FirstOrDefaultAsync(x => x.PersonId == personId);
            if (personResponse == null)
            {
                return false; ;
            }
             _db.Persons.Remove(_db.Persons.First(temp => temp.PersonId == personId));
            await _db.SaveChangesAsync();
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
            List<PersonResponse> persons = _db.Persons.Include("Country").Select(temp => temp.ToPersonResponse()).ToList();
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
                List<PersonResponse> response = _db.Persons.Include("Country").Select(temp => temp.ToPersonResponse()).ToList();
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
