using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class RoutesController : Controller
{
    private readonly IPricelistRepository _repository;

    public RoutesController(IPricelistRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index(string? origin, string? destination, string? company, string? sortBy)
    {
        //var legs = await _repository.GetFilteredLegsAsync(origin, destination, company, sortBy);
        var providers = await _repository.GetFilteredProvidersAsync(origin, destination, company, sortBy);

        var model = new RouteSearchViewModel
        {
            Origin = origin,
            Destination = destination,
            Company = company,
            SortBy = sortBy,
            Results = providers,
            AvailableOrigins = await _repository.GetAvailableOriginsAsync(),
            AvailableDestinations = await _repository.GetAvailableDestinationsAsync()
        };

        return View(model);
    }
}