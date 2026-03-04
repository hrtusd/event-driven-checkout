using EventDrivenCheckout.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventDrivenCheckout.Logistics.Services;

public class LogisticsService(
    ILogger<LogisticsService> logger,
    IPublishEndpoint publishEndpoint) : ILogisticsService
{
    public async Task ReserveShipment(OrderAccepted message)
    {
        var orderId = message.OrderId;

        logger.LogInformation("Shipment reserved {Id}", orderId);

        await publishEndpoint.Publish<ShipmentReserved>(new
        {
            OrderId = orderId,
        });

        await Task.Delay(3000);

        if (message.TriggerFailure)
        {
            logger.LogInformation("Shipment failed {Id}", orderId);

            await publishEndpoint.Publish<ShipmentFailed>(new
            {
                OrderId = orderId,
            });
        }
        else
        {
            var finalPrice = new Random().Next(59, 199);

            logger.LogInformation("Shipment repriced {Id}", orderId);

            await publishEndpoint.Publish<ShipmentRepriced>(new
            {
                OrderId = orderId,
                ShippingPrice = finalPrice
            });
        }
    }
}
