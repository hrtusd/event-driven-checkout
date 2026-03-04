using EventDrivenCheckout.Contracts;
using EventDrivenCheckout.Order.Consumers;
using EventDrivenCheckout.Order.Data;
using EventDrivenCheckout.Order.Data.Models;
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
            x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                    r.ExistingDbContext<OrderDbContext>();
                    r.UseSqlServer();
                });

            x.AddEntityFrameworkOutbox<OrderDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.AddConsumer<CreateOrderConsumer>();
            x.AddConsumer<ConfirmOrderConsumer>();
            x.AddConsumer<CancelOrderConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = builder.Configuration.GetConnectionString("messaging");
                cfg.Host(connectionString);

                cfg.Send<OrderAccepted>(x => x.UseCorrelationId(m => m.OrderId));
                cfg.Send<OrderConfirmed>(x => x.UseCorrelationId(m => m.OrderId));
                cfg.Send<OrderCancelled>(x => x.UseCorrelationId(m => m.OrderId));

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
