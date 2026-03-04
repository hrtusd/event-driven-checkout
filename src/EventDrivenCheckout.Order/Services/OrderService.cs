using EventDrivenCheckout.Contracts;
using EventDrivenCheckout.Order.Data;
using EventDrivenCheckout.Order.Data.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventDrivenCheckout.Order.Services;

internal class OrderService(
    OrderDbContext db,
    IPublishEndpoint publishEndpoint,
    ILogger<OrderService> logger) : IOrderService
{
    public Task CompleteOrderAsync(OrderShipped message)
    {
        throw new NotImplementedException();
    }

    public async Task CreateOrderAsync(CheckoutStarted message)
    {
        logger.LogInformation("Creating order {Id}", message.CorrelationId);

        var order = new Data.Models.Order
        {
            Id = message.CorrelationId,
            UserId = message.UserId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.PENDING_LOGISTICS,
            Items = [.. message.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Name = i.Name,
                Price = i.Price,
                Quantity = i.Quantity
            })]
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        logger.LogInformation("Order created {Id}", order.Id);

        await publishEndpoint.Publish(new OrderAccepted(order.Id, order.Id));
    }
}
