using MassTransit;

namespace EventDrivenCheckout.Order.Data.Models;

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = default!;
    public byte[] RowVersion { get; set; } = default!;
    public bool TriggerFailure { get; set; }
}
