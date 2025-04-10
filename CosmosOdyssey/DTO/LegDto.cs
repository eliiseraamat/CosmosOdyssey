using Domain;

namespace DTO;

public class LegDto : BaseEntity
{
    public RouteInfoDto RouteInfo { get; set; } = default!;
    
    public List<ProviderDto> Providers { get; set; } = default!;
}