using Domain;

namespace DTO;

public class RouteInfoDto : BaseEntity
{
    public PlanetDto From { get; set; } = default!;
    
    public PlanetDto To { get; set; } = default!;
    
    public double Distance { get; set; }
}