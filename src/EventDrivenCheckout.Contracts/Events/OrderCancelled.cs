namespace EventDrivenCheckout.Contracts.Events;

public interface OrderCancelled
{
    Guid OrderId { get; }
}