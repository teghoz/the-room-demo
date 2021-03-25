using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ServiceListAPI.IntegrationEvents.EventHandling;
using ServiceListAPI.IntegrationEvents.Events;
using TheRoom.BuildingBlocks.EventBus.Abstractions;

namespace ServiceListAPI.Extensions
{
    public static class EventBusConfigurationExtension
    {
        public static IApplicationBuilder ConfigureEventBus(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<ServiceListPriceChangedIntegrationEvent, ServiceListPriceChangeIntegrationEventHandler>();
            eventBus.Subscribe<BonusActivatedIntegrationEvent, BonsuActivatedIntegrationEventHandler>();
            return app;
        }
    }
}