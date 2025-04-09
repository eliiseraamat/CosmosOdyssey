namespace DTO;

public class PricelistDto
{
    public Guid Id { get; set; }
    
    public DateTime ValidUntil { get; set; }
    
    public List<LegDto> Legs { get; set; } = default!;
}