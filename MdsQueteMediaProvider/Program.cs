using Microsoft.Extensions.DependencyInjection;
using MdsProvider.TestService;
using System;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Configuration;
using System.IO;
using MdsProvider.Configuration;
using MdsProvider.ServiceMain.Controller;
using MdsProvider.TestService.API;

namespace MdsProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollingFile("MDS-Log-{Date}.txt")
                .CreateLogger();

            // create service collection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // create service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // entry to run app
            serviceProvider.GetService<App>().Run();

        }
        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // example of how to use the logger 
            Log.Logger.Debug("Adding services in ConfigureServices");

            // build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("app-settings.json", false)
                .Build();
            serviceCollection.AddOptions();
            serviceCollection.Configure<MdsProviderDiagnosticsConfig>(configuration.GetSection("MdsProviderDiagnosticsConfig")); // Adding the diagnostics configuration section 
            serviceCollection.Configure<MdsProviderDealerConfig>(configuration.GetSection("MdsProviderDealerConfig")); // Adding dealer configuration 
            serviceCollection.Configure<TestServiceConfig>(configuration.GetSection("TestServiceConfig")); // Configuration for the test service

            // add services
            serviceCollection.AddTransient<ITestService,TestService.Controller.TestService>();
            serviceCollection.AddSingleton<MdsProviderDealer>();

            // add app
            serviceCollection.AddTransient<App>();
        }

    }
}
