namespace ServiceListAPI.IntegrationEvents.EventHandling
{
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Serilog.Context;
    using ServiceListAPI.IntegrationEvents.Events;
    using System.Threading.Tasks;
    using TheRoom.BuildingBlocks.EventBus.Abstractions;

    public class OrderStatusChangedToPaidIntegrationEventHandler :
        IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
    {
        private readonly ServiceListContext _serviceListContext;
        private readonly ILogger<OrderStatusChangedToPaidIntegrationEventHandler> _logger;

        public OrderStatusChangedToPaidIntegrationEventHandler(
            ServiceListContext ServiceListContext,
            ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger)
        {
            _serviceListContext = ServiceListContext;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                //we're not blocking stock/inventory
                foreach (var orderStockItem in @event.OrderStockItems)
                {
                    var ServiceListItem = _serviceListContext.ServiceListItems.Find(orderStockItem.ProductId);

                    //ServiceListItem.RemoveStock(orderStockItem.Units);
                }

                await _serviceListContext.SaveChangesAsync();

            }
        }
    }
}