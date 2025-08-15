using CountriesService;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContract;
using ServiceContract.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonGetterServiceChild : PersonGetterService
    {
        public PersonGetterServiceChild(IPersonRepository personRepository,ILogger<PersonGetterService> logger):base(personRepository,logger)
        {
        }
        public async override Task<MemoryStream> GetPersonExcel()
        {
            MemoryStream memoryStream = new MemoryStream();//any type of file data csv,excel,image files
            using (ExcelPackage package = new ExcelPackage(memoryStream))//create a worksheet or workbook we use ExcelPackage
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add("PersonsSheet");
                workSheet.Cells["A1"].Value = "Person Name";
                workSheet.Cells["B1"].Value = "Age";
                workSheet.Cells["C1"].Value = "Gender";

                using (ExcelRange headercelss = workSheet.Cells["A1:C1"])
                {
                    headercelss.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headercelss.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headercelss.Style.Font.Bold = true;
                }
                int row = 2;
                List<PersonResponse> response = await GetAllPersons();
                foreach (PersonResponse responseItem in response)
                {
                    workSheet.Cells[row, 1].Value = responseItem.PersonName;
                    workSheet.Cells[row, 2].Value = responseItem.Age;
                    workSheet.Cells[row, 3].Value = responseItem.Gender;
                    row++;
                }
                workSheet.Cells[$"A1:C{row}"].AutoFitColumns();
                await package.SaveAsync();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
