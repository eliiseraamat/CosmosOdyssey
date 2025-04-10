using DAL;
using Microsoft.AspNetCore.Mvc;
using Services;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class RoutesController : Controller
{
    private readonly IPricelistRepository _repository;
    private readonly IRouteSortingService _sortingService;
    
    public RoutesController(IPricelistRepository repository, IRouteSortingService sortingService)
    {
        _repository = repository;
        _sortingService = sortingService;
    }

    public async Task<IActionResult> Index(string? origin, string? destination, string? company, string? sortBy)
    {
        var providers = await _sortingService.GetFilteredProvidersAsync(origin, destination, company, sortBy);

        var model = new RouteSearchViewModel
        {
            Origin = origin,
            Destination = destination,
            Company = company,
            SortBy = sortBy,
            Results = providers,
            AvailableOrigins = await _repository.GetAvailableOriginsAsync(),
            AvailableDestinations = await _repository.GetAvailableDestinationsAsync(),
        };
        
        if (TempData["ErrorMessage"] != null)
        {
            model.ErrorMessage = TempData["ErrorMessage"]!.ToString();
        }

        return View(model);
    }
}