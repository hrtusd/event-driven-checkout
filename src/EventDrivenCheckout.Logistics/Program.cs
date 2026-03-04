using EventDrivenCheckout.Contracts;
using EventDrivenCheckout.Logistics.Consumers;
using EventDrivenCheckout.Logistics.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventDrivenCheckout.Logistics;

internal class Program
{
    static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddScoped<ILogisticsService, LogisticsService>();

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderAcceptedConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = builder.Configuration.GetConnectionString("messaging");
                cfg.Host(connectionString);

                cfg.Send<ShipmentReserved>(x => x.UseCorrelationId(m => m.OrderId));
                cfg.Send<ShipmentRepriced>(x => x.UseCorrelationId(m => m.OrderId));
                cfg.Send<ShipmentFailed>(x => x.UseCorrelationId(m => m.OrderId));

                cfg.ConfigureEndpoints(context);
            });
        });

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddSource("MassTransit");
            });

        var host = builder.Build();
        host.Run();
    }
}