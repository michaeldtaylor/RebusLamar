using System;
using Microsoft.Extensions.Hosting;

namespace Overlord.Infrastructure
{
    public class HostEnvironmentAppNameProvider : IAppNameProvider
    {
        private readonly IHostEnvironment _environment;

        public HostEnvironmentAppNameProvider(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public string GetAppName()
        {
            if (string.IsNullOrEmpty(_environment.ApplicationName))
            {
                throw new Exception("IHostEnvironment.ApplicationName was not set in .NET Core Generic Host");
            }

            return _environment.ApplicationName;
        }
    }
}
