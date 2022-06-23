using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MyAmbassadorDemo.Function
{
    public class TimerTrigger1
    {
        [FunctionName("TimerTrigger1")]
        public async Task Run([TimerTrigger("*/10 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var job = new RespondToNewComments();
            await job.RunAsync();
        }
    }
}
