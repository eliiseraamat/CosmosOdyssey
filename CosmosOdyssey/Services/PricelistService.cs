using System.Text.Json;
using DAL;
using Domain;
using DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Services;

public class PricelistService(IServiceProvider serviceProvider, HttpClient httpClient) : BackgroundService
{
    private const string ApiUrl = "https://cosmosodyssey.azurewebsites.net/api/v1.0/TravelPrices";
    private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(1);
    private DateTime? _lastUpdatedTime;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("------");
                Console.WriteLine(_lastUpdatedTime);
                if (_lastUpdatedTime == null || _lastUpdatedTime <= DateTime.UtcNow)
                {
                    using var scope = serviceProvider.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IPricelistRepository>();
                
                    var response = await httpClient.GetAsync(ApiUrl, stoppingToken);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync(stoppingToken);
                    var dto = JsonSerializer.Deserialize<PricelistDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                    if (dto == null)
                    {
                        continue;
                    }
                    
                    if (!await repo.ExistsAsync(dto.Id))
                    {
                        var entity = await ConvertDtoToEntity(dto, repo);
                        await repo.AddPricelistAsync(entity);
                        
                        _lastUpdatedTime = dto.ValidUntil;
                        Console.WriteLine(".......");
                        Console.WriteLine(_lastUpdatedTime);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weather data: {ex.Message}");
            }
            
            await Task.Delay(PollInterval, stoppingToken);
        }
    }
    
    private async Task<Pricelist> ConvertDtoToEntity(PricelistDto dto, IPricelistRepository repo)
    {
        var pricelist = new Pricelist
        {
            Id = dto.Id,
            ValidUntil = dto.ValidUntil,
            Fetched = DateTime.UtcNow,
            Legs = new List<Leg>()
        };

        foreach (var legDto in dto.Legs)
        {
            var fromPlanet = await repo.GetOrCreatePlanetAsync(legDto.RouteInfo.From.Id, legDto.RouteInfo.From.Name);
            var toPlanet = await repo.GetOrCreatePlanetAsync(legDto.RouteInfo.To.Id, legDto.RouteInfo.To.Name);
            
            var routeInfo = new RouteInfo
            {
                Id = legDto.RouteInfo.Id,
                FromId = fromPlanet.Id,
                ToId = toPlanet.Id,
                Distance = legDto.RouteInfo.Distance
            };

            var leg = new Leg
            {
                Id = legDto.Id,
                PricelistId = dto.Id,
                RouteInfoId = routeInfo.Id,
                RouteInfo = routeInfo,
                Providers = new List<Provider>()
            };

            foreach (var providerDto in legDto.Providers)
            {
                var company = await repo.GetOrCreateCompanyAsync(providerDto.Company.Id, providerDto.Company.Name);

                var provider = new Provider
                {
                    Id = providerDto.Id,
                    CompanyId = company.Id,
                    FlightStart = providerDto.FlightStart,
                    FlightEnd = providerDto.FlightEnd,
                    Price = providerDto.Price
                };

                leg.Providers.Add(provider);
            }

            pricelist.Legs.Add(leg);
        }

        return pricelist;
    }
}