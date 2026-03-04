namespace EventDrivenCheckout.Contracts.Events;

public interface OrderConfirmed
{
    Guid OrderId { get; init; }
}