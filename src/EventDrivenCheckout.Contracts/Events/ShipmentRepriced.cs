namespace EventDrivenCheckout.Contracts.Events;

public interface ShipmentRepriced
{
    Guid OrderId { get; }
    decimal ShippingPrice { get; }
}