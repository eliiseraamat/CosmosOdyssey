namespace Domain;

public class ReservationProvider
{
    public Guid ReservationProviderId { get; set; }
    
    public Guid ProviderId { get; set; }
    public Provider? Provider { get; set; }
    
    public Guid ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
}