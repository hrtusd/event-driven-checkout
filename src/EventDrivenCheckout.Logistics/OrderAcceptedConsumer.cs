using EventDrivenCheckout.Contracts;
using MassTransit;

namespace EventDrivenCheckout.Logistics;

public class OrderAcceptedConsumer : IConsumer<OrderAccepted>
{
    public async Task Consume(ConsumeContext<OrderAccepted> context)
    {
        Console.WriteLine($"Logistics processing: {context.Message.OrderId}");

        await Task.Delay(1000);

        await context.Publish(new OrderShipped(context.Message.CorrelationId, context.Message.OrderId, DateTime.UtcNow));
    }
}
