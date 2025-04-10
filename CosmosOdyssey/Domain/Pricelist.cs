namespace Domain;

public class Pricelist : BaseEntity
{
    public DateTime ValidUntil { get; set; }
    
    public DateTime Fetched { get; set; }
    
    public ICollection<Leg>? Legs { get; set; }
}