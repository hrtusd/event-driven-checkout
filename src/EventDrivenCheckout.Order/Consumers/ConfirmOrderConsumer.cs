using EventDrivenCheckout.Order.Commands;
using EventDrivenCheckout.Order.Responses;
using EventDrivenCheckout.Order.Services;
using MassTransit;

namespace EventDrivenCheckout.Order.Consumers;

public class ConfirmOrderConsumer(IOrderService orderService) : IConsumer<ConfirmOrderCommand>
{
    public async Task Consume(ConsumeContext<ConfirmOrderCommand> context)
    {
        await orderService.CompleteOrderAsync(context.Message);

        await context.RespondAsync(new OrderConfirmedResponse(context.Message.OrderId));
    }
}
