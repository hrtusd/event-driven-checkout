using EventDrivenCheckout.Contracts.Events;
using EventDrivenCheckout.Order.Commands;
using EventDrivenCheckout.Order.Data.Models;
using EventDrivenCheckout.Order.Responses;
using MassTransit;

namespace EventDrivenCheckout.Order.StateMachine;

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
                    Hey = "Hi from V2"
                }))
                .TransitionTo(PENDING_LOGISTICS),
            When(CreateOrder.Faulted)
                .PublishAsync(context => context.Init<OrderCancelled>(new { OrderId = context.Saga.CorrelationId }))
                .TransitionTo(ORDER_CANCELLED)
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
                .TransitionTo(CONFIRMED),
            When(ConfirmOrder.Faulted)
                .Request(CancelOrder, context => new CancelOrderCommand(context.Saga.CorrelationId))
                .TransitionTo(CANCELLING)
        );

        During(CANCELLING,
            When(CancelOrder.Completed)
                .PublishAsync(context => context.Init<OrderCancelled>(new
                {
                    OrderId = context.Message.OrderId
                }))
                .TransitionTo(ORDER_CANCELLED),
            When(CancelOrder.Faulted)
                .PublishAsync(context => context.Init<OrderCancelled>(new { OrderId = context.Saga.CorrelationId }))
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
