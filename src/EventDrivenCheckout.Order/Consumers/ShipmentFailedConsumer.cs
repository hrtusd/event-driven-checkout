using EventDrivenCheckout.Contracts;
using EventDrivenCheckout.Order.Services;
using MassTransit;

namespace EventDrivenCheckout.Order.Consumers;

public class ShipmentFailedConsumer(IOrderService orderService) : IConsumer<ShipmentFailed>
{
    public Task Consume(ConsumeContext<ShipmentFailed> context) => orderService.CancelOrderAsync(context.Message);
}
