using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Person
    {
        [Key]
        public Guid PersonId { get; set; }
        //[Required]
        [StringLength(50)] 
        public string? PersonName { get; set; }
        [StringLength(50)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength (10)]
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        [StringLength(100)] 
        public string? Address { get; set; }
        public bool RecieveNewsLetters { get; set; }
        public string? TIN { get; set; }
        [ForeignKey("CountryId")]
        public Country? Country { get; set; }

    }
}
