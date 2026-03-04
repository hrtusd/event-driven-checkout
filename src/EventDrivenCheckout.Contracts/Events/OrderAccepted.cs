namespace EventDrivenCheckout.Contracts.Events;

public interface OrderAccepted
{
    Guid OrderId { get; init; }
    bool TriggerFailure { get; init; }
}

public interface OrderAcceptedV2 : OrderAccepted
{
    public string Hey { get; init; }
}