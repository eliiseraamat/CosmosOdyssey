using Domain;

namespace DAL;

public interface IPricelistRepository
{
    Task AddPricelistAsync(Pricelist pricelist);
    
    Task<bool> ExistsAsync(Guid apiId);
    
    Task<Company> GetOrCreateCompanyAsync(Guid id, string name);
    
    Task<Planet> GetOrCreatePlanetAsync(Guid id, string name);

    Task<List<string>> GetAvailableOriginsAsync();

    Task<List<string>> GetAvailableDestinationsAsync();

    Task<List<Provider>> GetFilteredProvidersAsync(
        string? origin, string? destination, string? company);

    Task<Provider?> GetProviderAsync(Guid legId, Guid providerId);
}