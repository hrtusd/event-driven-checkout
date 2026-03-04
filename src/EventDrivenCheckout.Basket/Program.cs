using EventDrivenCheckout.Contracts.Events;
using FastEndpoints;
using MassTransit;

namespace EventDrivenCheckout.Basket;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.Services.AddFastEndpoints();

        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        builder.AddRedisClient("cache");

        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = builder.Configuration.GetConnectionString("messaging");
                cfg.Host(connectionString);

                cfg.Send<BasketCheckedOut>(x => x.UseCorrelationId(m => m.CorrelationId));

                cfg.ConfigureEndpoints(context);
            });
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseFastEndpoints();

        app.Run();
    }
}
