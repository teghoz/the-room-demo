using Microsoft.Extensions.Logging;
using Serilog.Context;
using ServiceListAPI.Infrastructure;
using ServiceListAPI.IntegrationEvents.Events;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheRoom.BuildingBlocks.EventBus.Abstractions;
using TheRoom.BuildingBlocks.EventBus.Events;

namespace ServiceListAPI.IntegrationEvents.EventHandling
{
    public class BonsuActivatedIntegrationEventHandler :
        IIntegrationEventHandler<BonusActivatedIntegrationEvent>
    {
        private readonly ServiceListContext _serviceListContext;
        private readonly IServiceListIntegrationEventService _catalogIntegrationEventService;
        private readonly ILogger<BonsuActivatedIntegrationEventHandler> _logger;

        public BonsuActivatedIntegrationEventHandler(
            ServiceListContext ServiceListContext,
            IServiceListIntegrationEventService catalogIntegrationEventService,
            ILogger<BonsuActivatedIntegrationEventHandler> logger)
        {
            _serviceListContext = ServiceListContext;
            _catalogIntegrationEventService = catalogIntegrationEventService;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(BonusActivatedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                var confirmedIntegrationEvent = new BonusActivatedIntegrationEvent(@event.BonusId, @event.PromoId);
                await _catalogIntegrationEventService.SaveEventAndServiceListContextChangesAsync(confirmedIntegrationEvent);
                await _catalogIntegrationEventService.PublishThroughEventBusAsync(confirmedIntegrationEvent);

            }
        }
    }
}