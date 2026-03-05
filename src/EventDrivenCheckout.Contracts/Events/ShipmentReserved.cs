namespace EventDrivenCheckout.Contracts.Events;

public interface ShipmentReserved
{
    Guid OrderId { get; }
}