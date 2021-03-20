using TheRoom.BuildingBlocks.EventBus.Events;

namespace ServiceListAPI.IntegrationEvents.Events
{
    public record OrderStockConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public OrderStockConfirmedIntegrationEvent(int orderId) => OrderId = orderId;
    }
}