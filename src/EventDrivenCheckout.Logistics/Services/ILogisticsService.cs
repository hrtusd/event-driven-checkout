using EventDrivenCheckout.Contracts;

namespace EventDrivenCheckout.Logistics.Services;

public interface ILogisticsService
{
    public Task ReserveShipment(OrderAccepted message);
}
