using Microsoft.Extensions.DependencyInjection;

namespace Overlord.Other.ServiceProvider
{
    public abstract class ServiceRegistry
    {
        public abstract void Load(IServiceCollection services);
    }
}
