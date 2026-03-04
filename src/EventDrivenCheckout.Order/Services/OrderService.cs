using EventDrivenCheckout.Order.Commands;
using EventDrivenCheckout.Order.Data;
using EventDrivenCheckout.Order.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventDrivenCheckout.Order.Services;

internal class OrderService(
    OrderDbContext db,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task CreateOrderAsync(CreateOrderCommand message)
    {
        logger.LogInformation("Creating order {Id}", message.OrderId);

        var order = new Data.Models.Order
        {
            Id = message.OrderId,
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
    }

    public async Task CompleteOrderAsync(ConfirmOrderCommand message)
    {
        logger.LogInformation("Completing order {Id}", message.OrderId);

        var order = await db.Orders.Include(x => x.Items).FirstOrDefaultAsync(o => o.Id == message.OrderId);

        if (order != null)
        {
            order.Status = OrderStatus.CONFIRMED;
            order.ShippingPrice = message.ShippingPrice;
            order.TotalPrice = order.Items.Sum(i => i.Price * i.Quantity) + message.ShippingPrice;

            await db.SaveChangesAsync();

            logger.LogInformation("Order confirmed {Id}", message.OrderId);
        }
        else
        {
            logger.LogWarning("Order not found: {Id}", message.OrderId);
        }
    }

    public async Task CancelOrderAsync(CancelOrderCommand message)
    {
        logger.LogInformation("Cancelling order {Id}", message.OrderId);

        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == message.OrderId);

        if (order != null)
        {
            order.Status = OrderStatus.ORDER_CANCELLED;

            await db.SaveChangesAsync();

            logger.LogInformation("Order cancelled {Id}", message.OrderId);
        }
        else
        {
            logger.LogWarning("Order not found: {Id}", message.OrderId);
        }
    }
}
