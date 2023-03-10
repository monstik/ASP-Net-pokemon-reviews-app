using PokemonReviewApp.Models;

namespace PokemonReviewApp.interfaces;

public interface ICountryRepository
{
    ICollection<Country> GetCountries();

    Country GetCountry(int id);
    Country GetCountry(string name);
    Country GetCountryByOwner(int ownerId);

    ICollection<Owner> GetOwnersFromACountry(int countryId);
    bool CountryExists(int countryId);

    bool CreateCountry(Country country);
    bool UpdateCountry(Country country);
    bool DeleteCountry(Country country);
    bool Save();
}