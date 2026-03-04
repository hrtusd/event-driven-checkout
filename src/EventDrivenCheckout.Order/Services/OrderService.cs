using EventDrivenCheckout.Contracts;
using EventDrivenCheckout.Order.Data;
using EventDrivenCheckout.Order.Data.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventDrivenCheckout.Order.Services;

internal class OrderService(
    OrderDbContext db,
    IPublishEndpoint publishEndpoint,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task CompleteOrderAsync(ShipmentRepriced message)
    {
        logger.LogInformation("Order shipped {Id}", message.OrderId);

        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == message.OrderId);

        if (order != null)
        {
            order.Status = OrderStatus.CONFIRMED;
            order.ShippingPrice = message.ShippingPrice;

            await db.SaveChangesAsync();

            await publishEndpoint.Publish(new OrderConfirmed(order.Id));

            logger.LogInformation("Order confirmed {Id}", message.OrderId);
        }
        else
        {
            logger.LogWarning("Order not found: {Id}", message.OrderId);
        }
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

        await publishEndpoint.Publish(new OrderAccepted(order.Id));
    }
}
