using Entity;

namespace RepositoryContract
{
    /// <summary>
    /// Represrent data access logc for managinh package
    /// </summary>
    public interface ICountryRepository
    {
        Task<Country>  AddCountry(Country country);
        Task<List<Country>> GetAllCountries();
        Task<Country?> GetCountryByCOuntryId(Guid? CountryId);
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
