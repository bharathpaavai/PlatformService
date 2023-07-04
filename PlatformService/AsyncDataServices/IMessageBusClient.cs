using PlatformService.Dtos;

namespace PlatformService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void publishNewPlatform(PlatformPublishedDto platformPublishedDto);
        
    }
}
