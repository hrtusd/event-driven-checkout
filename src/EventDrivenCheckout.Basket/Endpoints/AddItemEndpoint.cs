using EventDrivenCheckout.Basket.Requests;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;
using System.Text.Json;

namespace EventDrivenCheckout.Basket.Endpoints;

[AllowAnonymous]
[HttpPost("/api/additem")]
public class AddItemEndpoint(IConnectionMultiplexer redis) : Endpoint<AddItemRequest>
{
    public override async Task HandleAsync(AddItemRequest request, CancellationToken cancellationToken)
    {
        var db = redis.GetDatabase();
        var key = $"basket:{request.UserId}";

        var json = await db.StringGetAsync(key);
        List<AddItemRequest> items = json.HasValue
            ? JsonSerializer.Deserialize<List<AddItemRequest>>(json.ToString()!)!
            : [];

        var existingItem = items.FirstOrDefault(i => i.ProductId == request.ProductId);

        if (existingItem != null)
        {
            var updatedItem = existingItem with { Quantity = existingItem.Quantity + request.Quantity };
            items.Remove(existingItem);
            items.Add(updatedItem);
        }
        else
        {
            items.Add(request);
        }

        await db.StringSetAsync(key, JsonSerializer.Serialize(items), TimeSpan.FromHours(24));

        await Send.OkAsync(items, cancellationToken);
    }
}
