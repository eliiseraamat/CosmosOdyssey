namespace Domain;

public class RouteInfo : BaseEntity
{
    public Guid FromId { get; set; }
    public Planet? From { get; set; }
    
    public Guid ToId { get; set; }
    public Planet? To { get; set; }

    public double Distance { get; set; }
    
    public ICollection<Leg>? Legs { get; set; }
}