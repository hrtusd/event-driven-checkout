using EventDrivenCheckout.Order.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EventDrivenCheckout.Order.Data;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Models.Order> Orders => Set<Models.Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderItem>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Models.Order>()
            .Property(x => x.ShippingPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Models.Order>()
            .Property(x => x.TotalPrice)
            .HasPrecision(18, 2);
    }
}
