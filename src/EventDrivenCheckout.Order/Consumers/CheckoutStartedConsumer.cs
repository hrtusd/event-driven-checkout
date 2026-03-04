using EventDrivenCheckout.Contracts;
using EventDrivenCheckout.Order.Services;
using MassTransit;

namespace EventDrivenCheckout.Order.Consumers;

public class CheckoutStartedConsumer(IOrderService orderService) : IConsumer<CheckoutStarted>
{
    public Task Consume(ConsumeContext<CheckoutStarted> context) => orderService.CreateOrderAsync(context.Message);
}
