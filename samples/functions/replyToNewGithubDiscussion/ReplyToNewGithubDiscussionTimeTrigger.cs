using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace APIMAuthorizationsDemo.Function
{
    public class ReplyToNewGithubDiscussionTimeTrigger
    {
        [FunctionName("ReplyToNewGithubDiscussionTimeTrigger")]
        public async Task Run([TimerTrigger("*/10 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"ReplyToNewGithubDiscussionTimeTrigger executed at: {DateTime.Now}");
            var job = new ReplyToNewGithubDiscussion();
            await job.RunAsync();
        }
    }
}
