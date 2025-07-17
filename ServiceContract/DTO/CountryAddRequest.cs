using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace ServiceContract.DTO
{
    /// <summary>
    /// Dto class fro adding new countries
    /// </summary>
    public class CountryAddRequest
    {
        public string? CountryName { get; set; }
        public Country ToCounty()
        {
            return new Country() { 
                CountryName = CountryName 
            };
        }
    }
}
