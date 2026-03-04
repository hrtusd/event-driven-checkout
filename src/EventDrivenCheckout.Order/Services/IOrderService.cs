using EventDrivenCheckout.Contracts;

namespace EventDrivenCheckout.Order.Services;

public interface IOrderService
{
    Task CreateOrderAsync(CheckoutStarted message);
    Task CompleteOrderAsync(ShipmentRepriced message);
}
