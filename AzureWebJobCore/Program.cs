using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace AzureWebJobCore
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    internal class Program
    {
        // https://github.com/Azure/azure-webjobs-sdk/issues/1940
        // https://github.com/Azure/azure-webjobs-sdk/issues/2088

        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        private static async Task Main()
        {
            var builder = new HostBuilder();
            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorage();
                //b.AddTimers();
            });
            var host = builder.Build();

            var cancellationToken = new WebJobsShutdownWatcher().Token;
            cancellationToken.Register(() =>
            {
                host.Services.GetService<IJobHost>().StopAsync();
                // bye bye
            });

            using (host)
            {
                await host.StartAsync(cancellationToken);

                var jobHost = host.Services.GetService<IJobHost>();

                await jobHost.CallAsync(nameof(Functions.LongRunningContinuousProcess),
                                     cancellationToken: cancellationToken);
            }
        }
    }
}