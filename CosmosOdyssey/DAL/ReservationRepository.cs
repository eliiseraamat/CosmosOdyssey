using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;
    private readonly IPricelistRepository _pricelistRepository;

    public ReservationRepository(AppDbContext context, IPricelistRepository pricelistRepository)
    {
        _context = context;
        _pricelistRepository = pricelistRepository;
    }

    public async Task<Reservation> CreateReservationAsync(Guid legId, Guid providerId, string firstName, string lastName)
    {
        var provider = await _pricelistRepository.GetProviderAsync(legId, providerId);

        // Create the reservation
        var reservation = new Reservation
        {
            FirstName = firstName,
            LastName = lastName,
            TotalPrice = provider!.Price,
            TotalTravelTime = provider.FlightEnd - provider.FlightStart,
        };

        // Create and link ReservationProvider
        var reservationProvider = new ReservationProvider
        {
            ProviderId = providerId,
            Reservation = reservation
        };

        reservation.ReservationProviders = new List<ReservationProvider> { reservationProvider };

        // Add the reservation and save changes
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return reservation;
    }

    public async Task<Reservation?> GetReservationByIdAsync(Guid reservationId)
    {
        return await _context.Reservations
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