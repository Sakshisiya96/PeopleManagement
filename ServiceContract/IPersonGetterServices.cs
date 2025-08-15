using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract.DTO;
using ServiceContract.Enums;

namespace ServiceContract
{
    /// <summary>
    /// Interface for retrieving person-related data.
    /// </summary>
    public interface IPersonGetterService
    {
        Task<List<PersonResponse>> GetAllPersons();
        Task<PersonResponse> GetPersonByPersonId(Guid? id);
        Task<List<PersonResponse>> GetFilteredPerson(string searchBy, string? seacrhString);
        Task<MemoryStream> GetPersonsCSV();
        /// <summary>
        /// Return person data in Excel format.
        /// </summary>
        /// <returns>MemoryStream containing Excel data.</returns>
        Task<MemoryStream> GetPersonExcel();
    }
}
