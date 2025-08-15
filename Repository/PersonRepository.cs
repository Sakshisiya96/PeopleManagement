using Entities;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Logging;
using RepositoryContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _db; 
        private readonly ILogger _logger;
        public PersonRepository(ApplicationDbContext db,ILogger<PersonRepository> logger)
        {
            _db = db;
            _logger= logger;
        }
        public async Task<Person> AddPersons(Person person)
        {
            _db.Persons.Add(person);
           await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid? id)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonId == id));
            int rowsCount = await _db.SaveChangesAsync();
            return rowsCount > 0;
        }
        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPerson(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation("GetFilteredPerson of Person Respoistory");
            return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid? id)
        {
           return await _db.Persons.Include("Country").
                FirstOrDefaultAsync(temp => temp.PersonId == id);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == person.PersonId);
            if (matchingPerson == null)
                return person;
            matchingPerson.PersonName=person.PersonName;
            matchingPerson.Email = person.Email;
            matchingPerson.Address = person.Address;
            matchingPerson.DateOfBirth = person.DateOfBirth;
            matchingPerson.Gender = person.Gender;
            matchingPerson.CountryId = person.CountryId;
            matchingPerson.RecieveNewsLetters = person.RecieveNewsLetters;
          //  _db.Persons.Update(matchingPerson);
           int countRows= await _db.SaveChangesAsync();
            return matchingPerson;

        }
    }
}
