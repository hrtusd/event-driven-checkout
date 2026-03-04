namespace EventDrivenCheckout.Contracts.Events;

public interface ShipmentRepriced
{
    Guid OrderId { get; init; }
    decimal ShippingPrice { get; init; }
}