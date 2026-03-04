using EventDrivenCheckout.Contracts;
using FastEndpoints;
using MassTransit;

namespace EventDrivenCheckout.Basket.Endpoints;

public class BasketCheckoutEndpoint(IPublishEndpoint publishEndpoint) : Endpoint<EmptyRequest>
{
    public override void Configure()
    {
        Post("/api/checkout");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var correlationId = Guid.NewGuid();

        await publishEndpoint.Publish(new CheckoutStarted(
            correlationId,
            "user-1234",
            [new("pid-1", "Stone", 100, 1)]), ct);

        await Send.OkAsync(new { CorrelationId = correlationId }, ct);
    }
}
