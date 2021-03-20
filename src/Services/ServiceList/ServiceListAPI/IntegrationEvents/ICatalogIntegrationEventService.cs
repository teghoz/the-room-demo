using TheRoom.BuildingBlocks.EventBus.Events;
using System.Threading.Tasks;

namespace ServiceListAPI.IntegrationEvents
{
    public interface ICatalogIntegrationEventService
    {
        Task SaveEventAndServiceListContextChangesAsync(IntegrationEvent evt);
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
