using Entities;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using RepositoryContract;

namespace Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext _context;
        public CountryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Country> AddCountry(Country country)
        {
           _context.Countries.Add(country);
            await _context.SaveChangesAsync();
            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
          return await _context.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryByCOuntryId(Guid? CountryId)
        {
           return await _context.Countries.FirstOrDefaultAsync(temp=>temp.CountryID == CountryId);
        }

        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
           return await _context.Countries.FirstOrDefaultAsync(temp => temp.CountryName == countryName);
        }
    }
}
