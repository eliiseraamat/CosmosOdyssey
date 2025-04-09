using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class PricelistRepository(AppDbContext context) : IPricelistRepository
{
    public async Task AddPricelistAsync(Pricelist pricelist)
    {
        await context.Pricelists.AddAsync(pricelist);
        
        var count = await context.Pricelists.CountAsync();

        if (count >= 15)
        {
            var oldest = await context.Pricelists.OrderBy(p => p.Fetched).FirstAsync();
            context.Pricelists.Remove(oldest);
        }
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid apiId)
    {
       return await context.Pricelists.AnyAsync(p => p.Id == apiId);
    }

    public async Task<Company> GetOrCreateCompanyAsync(Guid id, string name)
    {
        var existing = await context.Companies.FindAsync(id);
        if (existing != null) return existing;

        var company = new Company { Id = id, Name = name };
        context.Companies.Add(company);
        return company;
    }

    public async Task<Planet> GetOrCreatePlanetAsync(Guid id, string name)
    {
        var existing = await context.Planets.FindAsync(id);
        if (existing != null) return existing;

        var planet = new Planet { Id = id, Name = name };
        context.Planets.Add(planet);
        return planet;
    }

    public async Task<List<Leg>> GetFilteredLegsAsync(string? origin, string? destination, string? company, string? sortBy)
    {
        var now = DateTime.UtcNow;

        var query = context.Legs
            .Include(l => l.RouteInfo)
            .ThenInclude(r => r!.From)
            .Include(l => l.RouteInfo)
            .ThenInclude(r => r!.To)
            .Include(l => l.Providers)!
            .ThenInclude(p => p.Company)
            .Include(l => l.Pricelist)
            .Where(l => l.Pricelist!.ValidUntil > now)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(origin))
        {
            query = query.Where(l => l.RouteInfo!.From!.Name == origin);
        }

        if (!string.IsNullOrWhiteSpace(destination))
        {
            query = query.Where(l => l.RouteInfo!.To!.Name == destination);
        }

        if (!string.IsNullOrWhiteSpace(company))
        {
            query = query.Where(l => l.Providers!.Any(p => EF.Functions.Like(p.Company!.Name, $"%{company}%")));
        }

        var legs = await query.AsNoTracking().ToListAsync();

        // Apply global sorting across all results
        IOrderedEnumerable<Leg> sortedLegs = sortBy?.ToLower() switch
        {
            "price" => legs
                .OrderBy(l => l.Providers?
                    .Where(p => p != null)
                    .Select(p => (double)p!.Price)
                    .DefaultIfEmpty(double.MaxValue)
                    .Min() ?? double.MaxValue),
                
            "distance" => legs
                .OrderBy(l => l.RouteInfo?.Distance ?? double.MaxValue),
            
            "time" => legs
                .OrderBy(l => l.Providers?
                    .Where(p => p != null)
                    .Select(p => (p!.FlightEnd - p.FlightStart).TotalMinutes)
                    .DefaultIfEmpty(double.MaxValue)
                    .Min() ?? double.MaxValue),
            
            _ => legs.OrderBy(l => 0) // No sorting
        };

        return sortedLegs.ToList();

    }
    
    public async Task<List<Provider>> GetFilteredProvidersAsync(
        string? origin, string? destination, string? company, string? sortBy)
    {
        var now = DateTime.UtcNow;

        var query = context.Providers
            .Include(p => p.Company)
            .Include(p => p.Leg)!.ThenInclude(l => l!.RouteInfo)!.ThenInclude(r => r!.From)
            .Include(p => p.Leg)!.ThenInclude(l => l!.RouteInfo)!.ThenInclude(r => r!.To)
            .Include(p => p.Leg)!.ThenInclude(l => l!.Pricelist)
            .Where(p => p.Leg != null && p.Leg.Pricelist!.ValidUntil > now)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(origin))
        {
            query = query.Where(p => p.Leg!.RouteInfo!.From!.Name == origin);
        }

        if (!string.IsNullOrWhiteSpace(destination))
        {
            query = query.Where(p => p.Leg!.RouteInfo!.To!.Name == destination);
        }

        if (!string.IsNullOrWhiteSpace(company))
        {
            query = query.Where(p => p.Company!.Name.ToLower().Contains(company.ToLower()));
        }
        
        var providers = await query.ToListAsync();
        
        foreach (var p in providers)
        {
            var duration = (p.FlightEnd - p.FlightStart).TotalMinutes;
            Console.WriteLine($"{p.Company?.Name} - {duration} min - {p.FlightStart:HH:mm} → {p.FlightEnd:HH:mm}");
        }

        providers = sortBy?.ToLower() switch
        {
            "price" => providers.OrderBy(p => (double)p.Price).ToList(),
            "time" => providers.OrderBy(p => (p.FlightEnd - p.FlightStart).TotalMinutes).ToList(),
            "distance" => providers.OrderBy(p => p.Leg!.RouteInfo!.Distance).ToList(),
            _ => providers
        };

        return providers;
    }
    
    public async Task<List<string>> GetAvailableOriginsAsync()
    {
        var now = DateTime.UtcNow;

        return await context.Legs
            .Include(l => l.RouteInfo)
            .ThenInclude(r => r!.From)
            .Include(l => l.Pricelist)
            .Where(l => l.Pricelist!.ValidUntil > now)
            .Select(l => l.RouteInfo!.From!.Name)
            .Distinct()
            .OrderBy(n => n)
            .ToListAsync();
    }

    public async Task<List<string>> GetAvailableDestinationsAsync()
    {
        var now = DateTime.UtcNow;

        return await context.Legs
            .Include(l => l.RouteInfo)
            .ThenInclude(r => r!.To)
            .Include(l => l.Pricelist)
            .Where(l => l.Pricelist!.ValidUntil > now)
            .Select(l => l.RouteInfo!.To!.Name)
            .Distinct()
            .OrderBy(n => n)
            .ToListAsync();
    }

    public async Task<Leg?> GetLegAsync(Guid id)
    {
        return await context.Legs.FirstOrDefaultAsync(l => l.Id == id);
    }
    
    public async Task<Provider?> GetProviderAsync(Guid legId, Guid providerId)
    { 
        var provider = await context.Providers
            .Include(p => p.Leg)
            .ThenInclude(l => l!.Pricelist)
            .Include(p => p.Leg) // Include Leg
            .ThenInclude(l => l!.RouteInfo)
            .ThenInclude(r => r!.From)
            .Include(p => p.Leg)
            .ThenInclude(l => l!.RouteInfo)
            .ThenInclude(r => r!.To)
            .Include(p => p.Company) // Include Company
            .FirstOrDefaultAsync(p => p.Leg!.Id == legId && p.Id == providerId);

        return provider;
    }
}