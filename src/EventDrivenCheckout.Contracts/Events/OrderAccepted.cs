namespace EventDrivenCheckout.Contracts.Events;

public interface OrderAccepted
{
    Guid OrderId { get; }
    bool TriggerFailure { get; }
}

public interface OrderAcceptedV2 : OrderAccepted
{
    public string Hey { get; }
}