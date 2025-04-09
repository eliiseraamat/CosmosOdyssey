namespace DTO;

public class ProviderDto
{
    public Guid Id { get; set; }
    
    public CompanyDto Company { get; set; } = default!;
    
    public decimal Price { get; set; }
    
    public DateTime FlightStart { get; set; }
    
    public DateTime FlightEnd { get; set; }
}