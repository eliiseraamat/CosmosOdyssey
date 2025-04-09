using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Provider
{
    public Guid Id { get; set; }
    
    public Guid CompanyId { get; set; }
    public Company? Company { get; set; }
    
    public decimal Price { get; set; }
    
    public DateTime FlightStart { get; set; }
    
    public DateTime FlightEnd { get; set; }
    
    public Guid LegId { get; set; }
    public Leg? Leg { get; set; }
    
    public ICollection<ReservationProvider>? ReservationProviders { get; set; }
}