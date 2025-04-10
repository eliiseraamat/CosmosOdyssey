using Domain;

namespace WebApp.ViewModels;

public class ReservationViewModel
{
    public Guid LegId { get; set; }
    
    public Guid ProviderId { get; set; }
    public Leg Leg { get; set; } = null!;
    public Provider Provider { get; set; } = null!;
    public string CompanyName => Provider.Company!.Name;
}