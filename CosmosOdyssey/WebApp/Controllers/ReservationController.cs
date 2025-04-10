using DAL;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class ReservationController(
    IPricelistRepository pricelistPricelistRepository,
    IReservationRepository reservationRepository)
    : Controller
{
    public async Task<IActionResult> Index(Guid legId, Guid providerId)
    {
        var provider = await pricelistPricelistRepository.GetProviderAsync(legId, providerId);

        if (provider == null)
        { 
            TempData["ErrorMessage"] = "No available providers for this route.";
            return RedirectToAction("Index", "Routes");
        }
        
        if (provider.Leg == null || provider.Company == null)
        {
            TempData["ErrorMessage"] = "Provider data is incomplete.";
            return RedirectToAction("Index", "Routes");
        }

        var viewModel = new ReservationViewModel
        {
            Leg = provider.Leg!,
            Provider = provider
        };

        return View(viewModel);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Guid legId, Guid providerId, string firstName, string lastName)
    {
        var provider = await pricelistPricelistRepository.GetProviderAsync(legId, providerId);

        if (provider == null)
        {
            TempData["ErrorMessage"] = "No available providers for this route.";
            return RedirectToAction("Index", "Routes");
        }

        if (provider.Leg!.Pricelist!.ValidUntil < DateTime.UtcNow)
        {
            TempData["ErrorMessage"] = "Pricelist is expired.";
            return RedirectToAction("Index", "Routes");
        }
        
        var reservation = await reservationRepository.CreateReservationAsync(legId, providerId, firstName, lastName);

        return RedirectToAction("Confirmation", new { id = reservation.Id });
    }

    public async Task<IActionResult> Confirmation(Guid id)
    {
        var reservation = await reservationRepository.GetReservationByIdAsync(id);
        
        if (reservation == null)
        {
            TempData["ErrorMessage"] = "Reservation not found.";
            return RedirectToAction("Index", "Routes");
        }
        
        var reservationProvider = reservation.ReservationProviders?.FirstOrDefault();

        if (reservationProvider == null || reservationProvider.Provider == null)
        {
            TempData["ErrorMessage"] = "Provider not found in reservation.";
            return RedirectToAction("Index", "Routes");
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