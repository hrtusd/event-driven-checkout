using EventDrivenCheckout.Order.Commands;
using EventDrivenCheckout.Order.Services;
using MassTransit;

namespace EventDrivenCheckout.Order.Consumers;

public class CancelOrderConsumer(IOrderService orderService) : IConsumer<CancelOrderCommand>
{
    public async Task Consume(ConsumeContext<CancelOrderCommand> context)
    {
        await orderService.CancelOrderAsync(context.Message);

        await context.RespondAsync(new OrderCancelledResponse(context.Message.OrderId));
    }
}
