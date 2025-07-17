using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;
using ServiceContract.Enums;

namespace ServiceContract.DTO
{
    /// <summary>
    /// Represnet the DTO class that contain the person details to update
    /// </summary>
    public class PersonUpdateRequest
    {
        public Guid PersonId { get; set; }

        [Required(ErrorMessage = "Person Name can't be blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email canot be blank")]
        [EmailAddress(ErrorMessage = "Email value should be valid ")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Address { get; set; }
        public bool RecieveNewsLetters { get; set; }

        /// <summary>
        /// 
        /// Convert the current object of AddPersonRequest into Person object
        /// </summary>
        /// <returns></returns>
        public Person ToPerson()
        {
            return new Person()
            {
                PersonId = PersonId,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryId = CountryId,
                Address = Address,
                RecieveNewsLetters = RecieveNewsLetters
            };
        }
    }
}
