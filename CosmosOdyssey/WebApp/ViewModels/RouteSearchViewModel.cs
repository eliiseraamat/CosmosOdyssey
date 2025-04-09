using Domain;

namespace WebApp.ViewModels;

public class RouteSearchViewModel
{
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public string? Company { get; set; }
    public string? SortBy { get; set; }

    public List<Provider> Results { get; set; } = new();
    
    public List<string> AvailableOrigins { get; set; } = new();
    public List<string> AvailableDestinations { get; set; } = new();
}