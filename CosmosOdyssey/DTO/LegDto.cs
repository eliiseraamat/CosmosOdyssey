namespace DTO;

public class LegDto
{
    public Guid Id { get; set; }
    
    public RouteInfoDto RouteInfo { get; set; } = default!;
    
    public List<ProviderDto> Providers { get; set; } = default!;
}