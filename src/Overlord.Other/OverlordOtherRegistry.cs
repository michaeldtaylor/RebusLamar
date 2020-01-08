using Microsoft.Extensions.DependencyInjection;
using Overlord.Other.ServiceProvider;

namespace Overlord.Other
{
    public class OverlordOtherRegistry : ServiceRegistry
    {
        public override void Load(IServiceCollection services)
        {
            services.Scan(x => x
                .FromAssemblyOf<OverlordOtherRegistry>()
                .AddClasses()
                    .AsSelfWithInterfaces()
                    .WithTransientLifetime());
        }
    }
}
