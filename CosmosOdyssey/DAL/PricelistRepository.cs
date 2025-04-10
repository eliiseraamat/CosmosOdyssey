using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class PricelistRepository(AppDbContext context) : IPricelistRepository
{
    public async Task AddPricelistAsync(Pricelist pricelist)
    {
        await context.Pricelists.AddAsync(pricelist);
        
        var count = await context.Pricelists.CountAsync();

        if (count > 15)
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
    
    public async Task<List<Provider>> GetFilteredProvidersAsync(
        string? origin, string? destination, string? company)
    {
        var now = DateTime.UtcNow;

        var query = context.Providers
            .Include(p => p.Company)
            .Include(p => p.Leg).ThenInclude(l => l!.RouteInfo).ThenInclude(r => r!.From)
            .Include(p => p.Leg).ThenInclude(l => l!.RouteInfo).ThenInclude(r => r!.To)
            .Include(p => p.Leg).ThenInclude(l => l!.Pricelist)
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
        
        return await query.ToListAsync();
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
    
    public async Task<Provider?> GetProviderAsync(Guid legId, Guid providerId)
    { 
        return await context.Providers
            .Include(p => p.Leg)
            .ThenInclude(l => l!.Pricelist)
            .Include(p => p.Leg)
            .ThenInclude(l => l!.RouteInfo)
            .ThenInclude(r => r!.From)
            .Include(p => p.Leg)
            .ThenInclude(l => l!.RouteInfo)
            .ThenInclude(r => r!.To)
            .Include(p => p.Company)
            .FirstOrDefaultAsync(p => p.Leg!.Id == legId && p.Id == providerId);
    }
}