using Domain;

namespace DAL;

public interface IReservationRepository
{
    Task<Reservation> CreateReservationAsync(Guid legId, Guid providerId, string firstName, string lastName);
    
    Task<Reservation?> GetReservationByIdAsync(Guid reservationId);
}