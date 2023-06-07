using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        bool SaveChanges();
        IEnumerable<Platform> GatAllPlatforms();

        Platform GetPlatformById(int id);

        void createPlatform(Platform platform);

    }
}
