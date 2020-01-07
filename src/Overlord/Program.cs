using System;
using System.IO;
using System.Threading.Tasks;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Overlord.Modules.IncomingOrders;
using Rebus.NLog.Config;
using Rebus.Persistence.InMem;
using Rebus.Retry.Simple;
using Rebus.ServiceProvider;
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

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .UseLamar()
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
                    // Configure Rebus
                    services.AutoRegisterHandlersFromAssemblyOf<Program>();
                    services.AddRebus(c => c
                        .Options(o =>
                        {
                            o.SimpleRetryStrategy(maxDeliveryAttempts: 3, secondLevelRetriesEnabled: true);
                        })
                        .Logging(l => l.NLog())
                        .Subscriptions(s => s.StoreInMemory(new InMemorySubscriberStore()))
                        .Transport(t => t.UseInMemoryTransport(new InMemNetwork(true), "overlord-queue")));

                    // Configure Hosted Services
                    services.AddHostedService<OverlordService>();
                    services.AddHostedService<IncomingOrderPoller>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddNLog();
                })
                .ConfigureContainer((HostBuilderContext context, ServiceRegistry services) =>
                {
                    // Configure Lamar
                    services.Scan(x =>
                    {
                        x.AssembliesFromApplicationBaseDirectory();
                        x.LookForRegistries();
                    });
                });
    }
}
