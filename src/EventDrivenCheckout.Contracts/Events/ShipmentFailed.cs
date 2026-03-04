namespace EventDrivenCheckout.Contracts.Events;

public interface ShipmentFailed
{
    Guid OrderId { get; init; }
}