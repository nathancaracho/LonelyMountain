using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LonelyMountain.Src
{
    public class Bootstrap
    {
        public static void Start(Func<IServiceCollection, IServiceCollection> injector, string[] args) =>
         Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    injector(services)
                    .AddHostedService<Worker>())
                .Build()
                .Run();
    }
}
