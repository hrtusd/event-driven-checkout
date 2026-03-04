using EventDrivenCheckout.Contracts.Events;
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
                .PublishAsync(context => context.Init<OrderAcceptedV2>(new
                {
                    OrderId = context.Message.OrderId,
                    TriggerFailure = context.Saga.TriggerFailure,
                    Hey = "Hi"
                }))
                .TransitionTo(PENDING_LOGISTICS)
        );

        During(PENDING_LOGISTICS,
            When(ShipmentRepriced)
                .Request(ConfirmOrder, context => new ConfirmOrderCommand(context.Message.OrderId, context.Message.ShippingPrice))
                .TransitionTo(CONFIRMING),
            When(ShipmentFailed)
                .Request(CancelOrder, context => new CancelOrderCommand(context.Message.OrderId))
                .TransitionTo(CANCELLING)
        );

        During(CONFIRMING,
            When(ConfirmOrder.Completed)
                .PublishAsync(context => context.Init<OrderConfirmed>(new
                {
                    OrderId = context.Message.OrderId
                }))
                .TransitionTo(CONFIRMED)
        );

        During(CANCELLING,
            When(CancelOrder.Completed)
                .PublishAsync(context => context.Init<OrderCancelled>(new
                {
                    OrderId = context.Message.OrderId
                }))
                .TransitionTo(ORDER_CANCELLED)
        );
    }

    public State CREATING { get; private set; } = default!;
    public State PENDING_LOGISTICS { get; private set; } = default!;
    public State CONFIRMING { get; private set; } = default!;
    public State CONFIRMED { get; private set; } = default!;
    public State CANCELLING { get; private set; } = default!;
    public State ORDER_CANCELLED { get; private set; } = default!;

    public Event<BasketCheckedOut> CheckoutStarted { get; private set; } = default!;
    public Event<ShipmentRepriced> ShipmentRepriced { get; private set; } = default!;
    public Event<ShipmentFailed> ShipmentFailed { get; private set; } = default!;


    public Request<OrderState, CreateOrderCommand, OrderCreatedResponse> CreateOrder { get; private set; } = default!;
    public Request<OrderState, ConfirmOrderCommand, OrderConfirmedResponse> ConfirmOrder { get; private set; } = default!;
    public Request<OrderState, CancelOrderCommand, OrderCancelledResponse> CancelOrder { get; private set; } = default!;
}
