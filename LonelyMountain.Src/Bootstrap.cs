using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LonelyMountain.Src
{
    public static class Bootstrap
    {
        public static void Start(
            Func<IServiceCollection, IConfigurationRoot, IServiceCollection> injector
            , string[] args) =>
         Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    injector(services, GetConfigurationRoot())
                    .AddHostedService<Worker>())
                .Build()
                .Run();

        private static IConfigurationRoot GetConfigurationRoot() =>
         new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, true)
                    .AddEnvironmentVariables()
                    .Build();
    }
}
