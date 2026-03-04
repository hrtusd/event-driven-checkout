using EventDrivenCheckout.Contracts;
using EventDrivenCheckout.Order.Commands;
using EventDrivenCheckout.Order.Data.Models;
using MassTransit;

namespace EventDrivenCheckout.Order.Data;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => CheckoutStarted, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => ShipmentRepriced, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => ShipmentFailed, x => x.CorrelateById(m => m.Message.OrderId));

        Request(() => CreateOrder, x => x.Timeout = TimeSpan.Zero);
        Request(() => ConfirmOrder, x => x.Timeout = TimeSpan.Zero);
        Request(() => CancelOrder, x => x.Timeout = TimeSpan.Zero);

        Initially(
            When(CheckoutStarted)
                .Then(context =>
                {
                    context.Saga.TriggerFailure = context.Message.TriggerFailure;
                })
                .Request(CreateOrder, context => new CreateOrderCommand(context.Message.CorrelationId, context.Message.UserId, context.Message.Items))
                .TransitionTo(CREATING)
        );

        During(CREATING,
            When(CreateOrder.Completed)
        .TransitionTo(PENDING_LOGISTICS)
        .Publish(context => new OrderAccepted(context.Saga.CorrelationId, context.Saga.TriggerFailure))
        );

        During(PENDING_LOGISTICS,
            When(ShipmentRepriced)
                .TransitionTo(CONFIRMING)
                .Request(ConfirmOrder, context => new ConfirmOrderCommand(context.Message.OrderId, context.Message.ShippingPrice))
        );

        During(CONFIRMING,
            When(ConfirmOrder.Completed)
                .TransitionTo(CONFIRMED)
                .Publish(context => new OrderConfirmed(context.Message.OrderId))
        );

        During(PENDING_LOGISTICS,
            When(ShipmentFailed)
                .TransitionTo(CANCELLING)
                .Request(CancelOrder, context => new CancelOrderCommand(context.Message.OrderId))
        );

        During(CANCELLING,
            When(CancelOrder.Completed)
                .TransitionTo(ORDER_CANCELLED)
                .Publish(context => new OrderCancelled(context.Message.OrderId))
        );
    }

    public State CREATING { get; private set; } = default!;
    public State PENDING_LOGISTICS { get; private set; } = default!;
    public State CONFIRMING { get; private set; } = default!;
    public State CONFIRMED { get; private set; } = default!;
    public State CANCELLING { get; private set; } = default!;
    public State ORDER_CANCELLED { get; private set; } = default!;

    public Event<CheckoutStarted> CheckoutStarted { get; private set; } = default!;
    public Event<ShipmentRepriced> ShipmentRepriced { get; private set; } = default!;
    public Event<ShipmentFailed> ShipmentFailed { get; private set; } = default!;

    public Request<OrderState, CreateOrderCommand, OrderCreatedResponse> CreateOrder { get; private set; } = default!;
    public Request<OrderState, ConfirmOrderCommand, OrderConfirmedResponse> ConfirmOrder { get; private set; } = default!;
    public Request<OrderState, CancelOrderCommand, OrderCancelledResponse> CancelOrder { get; private set; } = default!;
}
