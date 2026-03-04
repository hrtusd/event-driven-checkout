using EventDrivenCheckout.Order.Commands;
using EventDrivenCheckout.Order.Services;
using MassTransit;

namespace EventDrivenCheckout.Order.Consumers;

public class CreateOrderConsumer(IOrderService orderService) : IConsumer<CreateOrderCommand>
{
    public async Task Consume(ConsumeContext<CreateOrderCommand> context)
    {
        await orderService.CreateOrderAsync(context.Message);

        await context.RespondAsync(new OrderCreatedResponse(context.Message.OrderId));
    }
}
