using EventDrivenCheckout.Order.Commands;

namespace EventDrivenCheckout.Order.Services;

public interface IOrderService
{
    Task CreateOrderAsync(CreateOrderCommand message);
    Task CompleteOrderAsync(ConfirmOrderCommand message);
    Task CancelOrderAsync(CancelOrderCommand message);
}
