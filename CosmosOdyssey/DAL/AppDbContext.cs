using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Pricelist> Pricelists { get; set; } = default!;
    public DbSet<Leg> Legs { get; set; } = default!;
    
    public DbSet<RouteInfo> RouteInfos { get; set; } = default!;
    public DbSet<Planet> Planets { get; set; } = default!;
    public DbSet<Provider> Providers { get; set; } = default!;
    public DbSet<Company> Companies { get; set; } = default!;
    public DbSet<Reservation> Reservations { get; set; } = default!;
    
    public DbSet<ReservationProvider> ReservationProviders { get; set; } = default!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Pricelist -> Legs (cascade)
        modelBuilder.Entity<Pricelist>()
            .HasMany(p => p.Legs)
            .WithOne(l => l.Pricelist)
            .HasForeignKey(l => l.PricelistId)
            .OnDelete(DeleteBehavior.Cascade);

        // Leg -> RouteInfo (cascade)
        modelBuilder.Entity<Leg>()
            .HasOne(l => l.RouteInfo)
            .WithOne()
            .HasForeignKey<Leg>(l => l.RouteInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Leg -> Providers (cascade)
        modelBuilder.Entity<Leg>()
            .HasMany(l => l.Providers)
            .WithOne(p => p.Leg)
            .HasForeignKey(p => p.LegId)
            .OnDelete(DeleteBehavior.Cascade);

        // Provider -> ReservationProviders (cascade)
        modelBuilder.Entity<Provider>()
            .HasMany(p => p.ReservationProviders)
            .WithOne(rp => rp.Provider)
            .HasForeignKey(rp => rp.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Reservation -> ReservationProviders (cascade)
        modelBuilder.Entity<Reservation>()
            .HasMany(r => r.ReservationProviders)
            .WithOne(rp => rp.Reservation)
            .HasForeignKey(rp => rp.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}