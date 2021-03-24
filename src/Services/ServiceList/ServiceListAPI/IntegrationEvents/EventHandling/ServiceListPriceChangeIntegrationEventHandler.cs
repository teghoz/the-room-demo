namespace ServiceListAPI.IntegrationEvents.EventHandling
{
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Serilog.Context;
    using ServiceListAPI.IntegrationEvents.Events;
    using System.Linq;
    using System.Threading.Tasks;
    using TheRoom.BuildingBlocks.EventBus.Abstractions;

    public class ServiceListPriceChangeIntegrationEventHandler :
        IIntegrationEventHandler<ServiceListPriceChangedIntegrationEvent>
    {
        private readonly ServiceListContext _serviceListContext;
        private readonly ILogger<ServiceListPriceChangeIntegrationEventHandler> _logger;

        public ServiceListPriceChangeIntegrationEventHandler(
            ServiceListContext ServiceListContext,
            ILogger<ServiceListPriceChangeIntegrationEventHandler> logger)
        {
            _serviceListContext = ServiceListContext;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ServiceListPriceChangedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                var serviceListItem = _serviceListContext.ServiceListItems.Where(l => l.Id == @event.ServiceListId).FirstOrDefault();
                serviceListItem.PriceAfterDiscount = @event.NewPrice;
                await _serviceListContext.SaveChangesAsync();

            }
        }
    }
}