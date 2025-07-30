using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContract
{
    public interface IPersonRepository
    {
        Task<Person> AddPersons(Person person);
        Task<List<Person>> GetAllPersons();
        Task<Person?> GetPersonByPersonId(Guid? id);

        /// <summary>
        /// Returns all person objects baseed on the given expression predicate
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All Matching persons with given conditions </returns>
        Task<List<Person>> GetFilteredPerson(Expression<Func<Person,bool>> predicate);
        Task<bool> DeletePersonByPersonId(Guid? id);
        Task<Person> UpdatePerson(Person person);

    }
}
