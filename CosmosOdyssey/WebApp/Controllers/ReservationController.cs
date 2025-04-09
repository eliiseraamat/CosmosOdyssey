using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class ReservationController : Controller
{
    private readonly IPricelistRepository _pricelistRepository;
    private readonly IReservationRepository _reservationReposiroty;

    public ReservationController(IPricelistRepository pricelistPricelistRepository, IReservationRepository reservationRepository)
    {
        _pricelistRepository = pricelistPricelistRepository;
        _reservationReposiroty = reservationRepository;
    }

    // GET: Reservation/Create/{legId}
    public async Task<IActionResult> Index(Guid legId, Guid providerId)
    {
        var provider = await _pricelistRepository.GetProviderAsync(legId, providerId);

        if (provider == null)
        {
            return NotFound("No available providers for this route.");
        }
        
        if (provider.Leg == null || provider.Company == null)
        {
            return NotFound("Provider data is incomplete.");
        }

        var viewModel = new ReservationViewModel
        {
            Leg = provider.Leg!,
            Provider = provider
        };

        return View(viewModel);
    }

    // POST: Reservation/Create/{legId}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Guid legId, Guid providerId, string firstName, string lastName)
    {
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            ModelState.AddModelError("", "All fields are required.");
            return View(); // Show the same page with validation errors
        }

        var provider = await _pricelistRepository.GetProviderAsync(legId, providerId);

        if (provider == null)
        {
            return NotFound("No available providers for this route.");
        }
        
        Console.WriteLine(provider.Leg!.Pricelist!.ValidUntil);
        Console.WriteLine(DateTime.UtcNow);

        if (provider.Leg!.Pricelist!.ValidUntil < DateTime.UtcNow)
        {
            return NotFound("Pricelist is expired.");
        }
        
        var reservation = await _reservationReposiroty.CreateReservationAsync(legId, providerId, firstName, lastName);
        
        // Redirect to confirmation page
        return RedirectToAction("Confirmation", new { id = reservation.Id });
    }

    // GET: Reservation/Confirmation/{id}
    public async Task<IActionResult> Confirmation(Guid id)
    {
        var reservation = await _reservationReposiroty.GetReservationByIdAsync(id);
        
        if (reservation == null)
        {
            return NotFound();
        }
        
        var reservationProvider = reservation.ReservationProviders?.FirstOrDefault();

        if (reservationProvider == null || reservationProvider.Provider == null)
        {
            return NotFound("Provider not found in reservation.");
        }
        
        var provider = reservationProvider.Provider;
        
        var viewModel = new ReservationViewModel
        {
            Leg = provider.Leg!,
            Provider = provider
        };
        
        return View(viewModel);
    }
}