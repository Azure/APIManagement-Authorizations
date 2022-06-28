using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace APIMAuthorizationsDemo.Function
{
    public class ReplyToNewGithubDiscussionTimeTrigger
    {
        [FunctionName("ReplyToNewGithubDiscussionTimeTrigger")]
        public async Task Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"ReplyToNewGithubDiscussionTimeTrigger started at: {DateTime.Now}");
            var apimServiceName = System.Environment.GetEnvironmentVariable("APIM_SERVICE_NAME", EnvironmentVariableTarget.Process);
            var subscriptionKey = System.Environment.GetEnvironmentVariable("SUBSCRIPTION_KEY", EnvironmentVariableTarget.Process);
            var githubRepoName = System.Environment.GetEnvironmentVariable("GITHUB_REPO_NAME", EnvironmentVariableTarget.Process);
            var githubRepoOwner = System.Environment.GetEnvironmentVariable("GITHUB_REPO_OWNER", EnvironmentVariableTarget.Process);
            var githubDiscussionNumber = Int32.Parse(System.Environment.GetEnvironmentVariable("GITHUB_DISCUSSION_NUMBER", EnvironmentVariableTarget.Process));
            var numSecondsForNewComments = Int32.Parse(System.Environment.GetEnvironmentVariable("NUM_SECONDS_FOR_NEW_COMMENTS", EnvironmentVariableTarget.Process));
            log.LogInformation($"ReplyToNewGithubDiscussionTimeTrigger read the following settings: "
                + $"APIM_SERVICE_NAME={apimServiceName}, GITHUB_REPO_NAME={githubRepoName}, GITHUB_REPO_OWNER={githubRepoOwner}, GITHUB_REPO_OWNER={githubDiscussionNumber}, NUM_SECONDS_FOR_NEW_COMMENTS={numSecondsForNewComments}");

            var job = new ReplyToNewGithubDiscussion(log, apimServiceName, subscriptionKey, githubRepoName, githubRepoOwner, githubDiscussionNumber, numSecondsForNewComments);
            await job.RunAsync();
        }
    }
}
