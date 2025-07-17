using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace ServiceContract.DTO
{
    public class CountryResponse
    {
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj==null)
            {
                return false;
            }
            if (obj.GetType() != typeof(CountryResponse))
            {
                return false;
            }
            CountryResponse other = (CountryResponse)obj;
            return CountryId == other.CountryId && CountryName == other.CountryName;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public static class CountryResponseExtesion
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            //counvert country to country response object
            return new CountryResponse()
            {
                CountryId = country.CountryID,
                CountryName = country.CountryName,
            };
        }
    }
}
