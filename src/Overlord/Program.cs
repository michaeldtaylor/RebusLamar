using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Overlord.Modules.IncomingOrders;
using Rebus.Config;
using Rebus.NLog.Config;
using Rebus.Persistence.InMem;
using Rebus.Retry.Simple;
using Rebus.Transport.InMem;

namespace Overlord
{
    public class Program
    {
        private const string AppName = "overlord";

        public static async Task Main(string[] args)
        {
            Console.Title = AppName;

            var host = CreateHostBuilder(args).Build();

            var hostedServices = host.Services.GetServices<IHostedService>();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseConsoleLifetime()
                .ConfigureHostConfiguration(config =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());

                    config.AddEnvironmentVariables("DOTNET_");
                    config.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    context.HostingEnvironment.ApplicationName = AppName;

                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddOptions();
                    services.AddHostedService<OverlordService>();
                    services.AddHostedService<IncomingOrderPoller>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddNLog();
                })
                .ConfigureContainer((HostBuilderContext context, ContainerBuilder builder) =>
                {
                    builder.RegisterModule<OverlordModule>();

                    // Configure Rebus
                    builder.RegisterRebus((c, ctx) => c
                        .Options(o =>
                        {
                            o.SimpleRetryStrategy(maxDeliveryAttempts: 3, secondLevelRetriesEnabled: true);
                        })
                        .Logging(l => l.NLog())
                        .Subscriptions(s => s.StoreInMemory(new InMemorySubscriberStore()))
                        .Transport(t => t.UseInMemoryTransport(new InMemNetwork(true), "overlord-queue")));
                });
    }
}
