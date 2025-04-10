namespace Domain;

public class Leg : BaseEntity
{
    public Guid RouteInfoId { get; set; }
    public RouteInfo? RouteInfo { get; set; }
    
    public Guid PricelistId { get; set; }
    public Pricelist? Pricelist { get; set; }
    
    public ICollection<Provider>? Providers { get; set; }
}