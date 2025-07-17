using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Entity;
using ServiceContract.Enums;

namespace ServiceContract.DTO
{
    /// <summary>
    /// Represent the DTO class thart is used as return type of most method of Person 
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Country { get; set; }//get country name on the basis of country id
        public string? Address { get; set; }
        public double? Age { get; set; }
        public bool RecieveNewsLetters { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(PersonResponse)) return false;
            PersonResponse other = (PersonResponse)obj;
            return PersonId == other.PersonId && PersonName == other.PersonName
                && Email == other.Email
                && DateOfBirth == other.DateOfBirth
                && Gender == other.Gender
                && CountryId == other.CountryId
                && Address == other.Address
                && Age == other.Age
                && Country == other.Country
                && RecieveNewsLetters == other.RecieveNewsLetters;

        }
        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonId = PersonId,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),//convert string to enum
                Address = Address,
                CountryId = CountryId,
                RecieveNewsLetters = RecieveNewsLetters,

            };
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
    public static class PersonExtension
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonId = person.PersonId,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryId = person.CountryId,
                Address = person.Address,
                RecieveNewsLetters = person.RecieveNewsLetters,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
                Country = person.Country?.CountryName
            };


        }
    }

}
