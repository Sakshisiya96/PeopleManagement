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
    public class PersonGetterService : IPersonGetterService
    {
        private readonly IPersonRepository _personsRespository;
      
        private readonly ILogger<PersonGetterService> _logger;
        public PersonGetterService(IPersonRepository personRepostory,ILogger<PersonGetterService> logger)
        {
            _personsRespository = personRepostory;
            _logger = logger;
           
      
        }
       
        public virtual async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of Persons Service");
            var person = await _personsRespository.GetAllPersons();
            return  person //select * from persons
                .Select(temp => temp.ToPersonResponse()).ToList();//list of person return

            //return _db.sp_GetAllPersons()
            // .Select(temp => ToPersonResponses(temp)).ToList();//list of person return
        }

        public virtual async Task<PersonResponse> GetPersonByPersonId(Guid? personId)
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

        public virtual async Task<List<PersonResponse>> GetFilteredPerson(string searchBy, string? seacrhString)
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

        public virtual async Task<MemoryStream> GetPersonsCSV()
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

        public virtual async Task<MemoryStream> GetPersonExcel()
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
