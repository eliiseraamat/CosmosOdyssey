using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Reservation
{
    public Guid Id { get; set; }
    
    [MaxLength(128)]
    public string FirstName { get; set; } = default!;
    
    [MaxLength(128)]
    public string LastName { get; set; } = default!;
    
    public decimal TotalPrice { get; set; }
    
    public TimeSpan TotalTravelTime { get; set; }
    
    public ICollection<ReservationProvider>? ReservationProviders { get; set; }
}