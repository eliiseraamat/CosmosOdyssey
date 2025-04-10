using Domain;

namespace DTO;

public class ProviderDto : BaseEntity
{
    public CompanyDto Company { get; set; } = default!;
    
    public decimal Price { get; set; }
    
    public DateTime FlightStart { get; set; }
    
    public DateTime FlightEnd { get; set; }
}