
using Entity;
using ServiceContract.Enums;
using System.ComponentModel.DataAnnotations;
namespace ServiceContract.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage ="Person Name can't be blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage ="Email canot be blank")]
        [EmailAddress(ErrorMessage ="Email value should be valid ")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public GenderOptions? Gender { get; set; }
        [Required(ErrorMessage="Please select country")]
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
