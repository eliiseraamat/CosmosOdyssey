using DAL;

namespace Services;

public class RouteSearchService
{
    private readonly AppDbContext _context;

    public RouteSearchService(AppDbContext context)
    {
        _context = context;
    }
}