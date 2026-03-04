using EventDrivenCheckout.Order.Consumers;
using EventDrivenCheckout.Order.Data;
using EventDrivenCheckout.Order.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventDrivenCheckout.Order;

internal class Program
{
    static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddScoped<IOrderService, OrderService>();

        builder.AddSqlServerDbContext<OrderDbContext>("OrderDb");

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<CheckoutStartedConsumer>();
            x.AddConsumer<OrderShippedConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = builder.Configuration.GetConnectionString("messaging");

                cfg.Host(connectionString);

                cfg.ConfigureEndpoints(context);
            });
        });

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddSource("MassTransit")
                       .AddSource("Microsoft.EntityFrameworkCore.SqlServer");
            });

        var host = builder.Build();


        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        db.Database.Migrate();


        host.Run();
    }
}
