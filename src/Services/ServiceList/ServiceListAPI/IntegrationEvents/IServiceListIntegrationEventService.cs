using System.Threading.Tasks;
using TheRoom.BuildingBlocks.EventBus.Events;

namespace ServiceListAPI.IntegrationEvents
{
    public interface IServiceListIntegrationEventService
    {
        Task SaveEventAndServiceListContextChangesAsync(IntegrationEvent evt);
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
