using EventDrivenCheckout.Contracts;
using EventDrivenCheckout.Logistics.Services;
using MassTransit;

namespace EventDrivenCheckout.Logistics.Consumers;

public class OrderAcceptedConsumer(ILogisticsService logisticsService) : IConsumer<OrderAccepted>
{
    public Task Consume(ConsumeContext<OrderAccepted> context) => logisticsService.ReserveShipment(context.Message);
}
