using Domain;

namespace Services;

public interface IRouteSortingService
{
    Task<List<Provider>> GetFilteredProvidersAsync(string? origin, string? destination, string? company, string? sortBy);
}