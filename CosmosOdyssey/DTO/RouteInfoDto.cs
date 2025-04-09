namespace DTO;

public class RouteInfoDto
{
    public Guid Id { get; set; }
    
    public PlanetDto From { get; set; } = default!;
    
    public PlanetDto To { get; set; } = default!;
    
    public double Distance { get; set; }
}