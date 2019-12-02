using Microsoft.Azure.WebJobs;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AzureWebJobCore
{
    public class Functions
    {
        [FunctionName(nameof(Functions.LongRunningContinuousProcess))]
        [NoAutomaticTrigger]
        public async Task LongRunningContinuousProcess(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch

                try
                {
                    Console.WriteLine($"Start: {DateTime.UtcNow}");

                    ///
                    /// Do work here!
                    ///
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.ToString()}");
                }
                finally
                {
                    // load all and process
                    Console.WriteLine($"Stop: {DateTime.UtcNow}");
                    stopwatch.Stop();

                    var totalSeconds = (int)stopwatch.Elapsed.TotalSeconds;
                    Console.WriteLine($"Total Seconds Processed: {totalSeconds}");

                    // wait an hour for next run. ensure to update azure configuration property though: WEBJOBS_IDLE_TIMEOUT
                    await Task.Delay(3600000);
                }
            }
        }
    }
}