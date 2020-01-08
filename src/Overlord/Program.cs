using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Overlord.Domain.Messages;
using Overlord.Modules.IncomingOrders;
using Overlord.Other.ServiceProvider;
using Rebus.Handlers;
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

            var hostedServices = host.Services.GetServices<IHostedService>();
            var handlers = host.Services.GetServices<IHandleMessages<GenerateProductVariant>>();
            var failedHandlers = host.Services.GetServices<IHandleMessages<IFailed<GenerateProductVariant>>>();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
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
                    // Add Registries
                    services.AddRegistries<Program>(r => r.Name.StartsWith("Overlord"));

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
                });
    }
}
