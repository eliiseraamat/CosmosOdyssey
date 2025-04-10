using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ReservationRepository(AppDbContext context, IPricelistRepository pricelistRepository)
    : IReservationRepository
{
    public async Task<Reservation> CreateReservationAsync(Guid legId, Guid providerId, string firstName, string lastName)
    {
        var provider = await pricelistRepository.GetProviderAsync(legId, providerId);
        
        var reservation = new Reservation
        {
            FirstName = firstName,
            LastName = lastName,
            TotalPrice = provider!.Price,
            TotalTravelTime = provider.FlightEnd - provider.FlightStart,
        };
        
        var reservationProvider = new ReservationProvider
        {
            ProviderId = providerId,
            Reservation = reservation
        };

        reservation.ReservationProviders = new List<ReservationProvider> { reservationProvider };
        
        context.Reservations.Add(reservation);
        await context.SaveChangesAsync();

        return reservation;
    }

    public async Task<Reservation?> GetReservationByIdAsync(Guid reservationId)
    {
        return await context.Reservations
            .Include(r => r.ReservationProviders)!
            .ThenInclude(p => p.Provider)
            .ThenInclude(p => p!.Leg)
            .ThenInclude(l => l!.RouteInfo)
            .ThenInclude(r => r!.From)
            .Include(r => r.ReservationProviders)!
            .ThenInclude(p => p.Provider)
            .ThenInclude(p => p!.Leg)
            .ThenInclude(l => l!.RouteInfo)
            .ThenInclude(r => r!.To)
            .Include(r => r.ReservationProviders)!
            .ThenInclude(p => p.Provider)
            .ThenInclude(p => p!.Company)
            .FirstOrDefaultAsync(r => r.Id == reservationId);
    }
}