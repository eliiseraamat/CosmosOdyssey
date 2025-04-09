namespace Domain;

public class Pricelist
{
    public Guid Id { get; set; }
    
    public DateTime ValidUntil { get; set; }
    
    public DateTime Fetched { get; set; }
    
    public ICollection<Leg>? Legs { get; set; }
}