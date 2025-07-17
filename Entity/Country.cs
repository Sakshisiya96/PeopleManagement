using System.ComponentModel.DataAnnotations;

namespace Entity
{
    /// <summary>
    /// Doamin model for stroing country
    /// </summary>
    public class Country
    {
        [Key]
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }
        public virtual ICollection<Person>? persons { get; set; }


    }
}
