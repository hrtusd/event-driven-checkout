using EventDrivenCheckout.Contracts;
using EventDrivenCheckout.Order.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventDrivenCheckout.Order.Consumers;

public class OrderShippedConsumer(
    OrderDbContext dbContext,
    ILogger<OrderShippedConsumer> logger) : IConsumer<OrderShipped>
{
    public async Task Consume(ConsumeContext<OrderShipped> context)
    {
        var message = context.Message;
        logger.LogInformation("Order shipped {Id}", message.OrderId);

        var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == message.OrderId);

        if (order != null)
        {
            order.Status = OrderStatus.CONFIRMED;
            order.ShippedAt = message.ShippedAt;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Order confirmed {Id}", message.OrderId);
        }
        else
        {
            logger.LogWarning("Order not found: {Id}", message.OrderId);
        }
    }
}
