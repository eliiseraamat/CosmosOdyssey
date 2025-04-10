using DAL;
using Domain;

namespace Services;

public class RouteSortingService(IPricelistRepository repository) : IRouteSortingService
{
    public async Task<List<Provider>> GetFilteredProvidersAsync(string? origin, string? destination, string? company, string? sortBy)
    {
        var providers = await repository.GetFilteredProvidersAsync(origin, destination, company);
        
        providers = sortBy?.ToLower() switch
        {
            "price" => providers.OrderBy(p => (double)p.Price).ToList(),
            "time" => providers.OrderBy(p => (p.FlightEnd - p.FlightStart).TotalMinutes).ToList(),
            "distance" => providers.OrderBy(p => p.Leg!.RouteInfo!.Distance).ToList(),
            _ => providers
        };

        return providers;
    }
}