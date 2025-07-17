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
    /// 
    /// </summary>
    public interface IPersonService
    {
        Task<PersonResponse> AddPerson(PersonAddRequest? request);
        Task<List<PersonResponse>> GetAllPersons();
        Task<PersonResponse> GetPersonByPersonId(Guid? id);
        Task<List<PersonResponse>> GetFilteredPerson(string searchBy, string? seacrhString);
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SeacrhOrderOption SortOrder);
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? request);
        Task<bool> DeletePerson(Guid? id);
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Persons as csv</returns>
        Task<MemoryStream> GetPersonsCSV();
        /// <summary>
        /// Retrun person on Excel
        /// </summary>
        /// <returns></returns>
        Task<MemoryStream> GetPersonExcel();
    }
}
