using EventDrivenCheckout.Order.Data.Models;
using EventDrivenCheckout.Order.StateMachine;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace EventDrivenCheckout.Order.Data;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : SagaDbContext(options)
{
    public DbSet<Models.Order> Orders => Set<Models.Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new OrderStateMap(); }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();

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
