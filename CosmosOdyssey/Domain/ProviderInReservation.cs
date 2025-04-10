namespace Domain;

public class ReservationProvider : BaseEntity
{
    public Guid ProviderId { get; set; }
    public Provider? Provider { get; set; }
    
    public Guid ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
}