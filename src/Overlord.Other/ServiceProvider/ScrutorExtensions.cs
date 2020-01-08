using Microsoft.Extensions.Hosting;
using Scrutor;

namespace Overlord.Other.ServiceProvider
{
    public static class ScrutorExtensions
    {
        public static IServiceTypeSelector AddNonCoreClasses(this IImplementationTypeSelector implementationTypeSelector)
        {
            return implementationTypeSelector.AddClasses(i => i.Where(t => !typeof(IHostedService).IsAssignableFrom(t)));
        }
    }
}
