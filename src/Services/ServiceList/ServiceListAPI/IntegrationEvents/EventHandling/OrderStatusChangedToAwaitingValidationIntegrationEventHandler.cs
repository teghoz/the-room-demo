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
    public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler :
        IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>
    {
        private readonly ServiceListContext _serviceListContext;
        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;
        private readonly ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> _logger;

        public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(
            ServiceListContext ServiceListContext,
            ICatalogIntegrationEventService catalogIntegrationEventService,
            ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger)
        {
            _serviceListContext = ServiceListContext;
            _catalogIntegrationEventService = catalogIntegrationEventService;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();

                foreach (var orderStockItem in @event.OrderStockItems)
                {
                    var ServiceListItem = _serviceListContext.ServiceListItems.Find(orderStockItem.ProductId);
                    var confirmedOrderStockItem = new ConfirmedOrderStockItem(ServiceListItem.Id, false);

                    confirmedOrderStockItems.Add(confirmedOrderStockItem);
                }

                var confirmedIntegrationEvent = confirmedOrderStockItems.Any(c => !c.HasStock)
                    ? (IntegrationEvent)new OrderStockRejectedIntegrationEvent(@event.OrderId, confirmedOrderStockItems)
                    : new OrderStockConfirmedIntegrationEvent(@event.OrderId);

                await _catalogIntegrationEventService.SaveEventAndServiceListContextChangesAsync(confirmedIntegrationEvent);
                await _catalogIntegrationEventService.PublishThroughEventBusAsync(confirmedIntegrationEvent);

            }
        }
    }
}