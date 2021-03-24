namespace ServiceListAPI.IntegrationEvents.Events
{
    using TheRoom.BuildingBlocks.EventBus.Events;

    public record BonusActivatedIntegrationEvent : IntegrationEvent
    {
        public int BonusId { get; }
        public int PromoId { get; }

        public BonusActivatedIntegrationEvent(int bonusId, int promoId)
        {
            BonusId = bonusId;
            PromoId = promoId;
        }
    }
}