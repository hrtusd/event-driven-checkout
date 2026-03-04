using EventDrivenCheckout.Contracts;
using EventDrivenCheckout.Order.Services;
using MassTransit;

namespace EventDrivenCheckout.Order.Consumers;

public class ShipmentRepricedConsumer(IOrderService orderService) : IConsumer<ShipmentRepriced>
{
    public Task Consume(ConsumeContext<ShipmentRepriced> context) => orderService.CompleteOrderAsync(context.Message);
}
