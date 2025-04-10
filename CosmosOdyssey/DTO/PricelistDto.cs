using Domain;

namespace DTO;

public class PricelistDto : BaseEntity
{
    public DateTime ValidUntil { get; set; }
    
    public List<LegDto> Legs { get; set; } = default!;
}