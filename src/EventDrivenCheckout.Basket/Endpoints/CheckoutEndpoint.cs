using EventDrivenCheckout.Basket.Requests;
using EventDrivenCheckout.Contracts;
using EventDrivenCheckout.Contracts.Events;
using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;
using System.Text.Json;

namespace EventDrivenCheckout.Basket.Endpoints;

[AllowAnonymous]
[HttpPost("/api/checkout")]
public class CheckoutEndpoint(
    IConnectionMultiplexer redis,
    IPublishEndpoint publishEndpoint) : Endpoint<CheckoutRequest>
{
    public override async Task HandleAsync(CheckoutRequest request, CancellationToken cancellationToken)
    {
        var db = redis.GetDatabase();
        var key = $"basket:{request.UserId}";

        var json = await db.StringGetAsync(key);
        if (!json.HasValue)
        {
            await Send.ErrorsAsync(400, cancellationToken);
            return;
        }

        var items = JsonSerializer.Deserialize<List<AddItemRequest>>(json.ToString()!)!;

        var correlationId = Guid.NewGuid();
        await publishEndpoint.Publish<BasketCheckedOut>(new
        {
            CorrelationId = correlationId,
            Items = (List<BasketItem>)[.. items.Select(i => new BasketItem(i.ProductId, i.Name, i.Price, i.Quantity))],
            request.TriggerFailure,
            request.UserId
        }, cancellationToken);

        await db.KeyDeleteAsync(key);

        await Send.OkAsync(new { CorrelationId = correlationId }, cancellationToken);
    }
}
