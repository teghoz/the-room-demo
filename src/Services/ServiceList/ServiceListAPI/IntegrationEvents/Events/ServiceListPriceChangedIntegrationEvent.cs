namespace ServiceListAPI.IntegrationEvents.Events
{
    using TheRoom.BuildingBlocks.EventBus.Events;

    public record ServiceListPriceChangedIntegrationEvent : IntegrationEvent
    {
        public int ServiceListId { get; private init; }

        public decimal NewPrice { get; private init; }

        public decimal OldPrice { get; private init; }

        public ServiceListPriceChangedIntegrationEvent(int serviceListId, decimal newPrice, decimal oldPrice)
        {
            ServiceListId = serviceListId;
            NewPrice = newPrice;
            OldPrice = oldPrice;
        }
    }
}
