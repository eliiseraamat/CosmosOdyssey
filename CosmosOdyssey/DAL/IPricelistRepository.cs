using Domain;
using DTO;

namespace DAL;

public interface IPricelistRepository
{
    Task AddPricelistAsync(Pricelist pricelist);
    
    Task<bool> ExistsAsync(Guid apiId);
    
    Task<Company> GetOrCreateCompanyAsync(Guid id, string name);
    
    Task<Planet> GetOrCreatePlanetAsync(Guid id, string name);
    
    Task<List<Leg>> GetFilteredLegsAsync(string? origin, string? destination, string? companyName, string? sortBy);

    Task<List<string>> GetAvailableOriginsAsync();

    Task<List<string>> GetAvailableDestinationsAsync();

    Task<Leg?> GetLegAsync(Guid id);

    Task<List<Provider>> GetFilteredProvidersAsync(
        string? origin, string? destination, string? company, string? sortBy);

    Task<Provider?> GetProviderAsync(Guid legId, Guid providerId);
}