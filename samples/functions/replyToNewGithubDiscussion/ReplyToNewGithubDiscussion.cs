using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace APIMAuthorizationsDemo.Function
{
  public class ReplyToNewGithubDiscussion {
    private readonly ILogger _log;
    private ApimService _apimService;
    private string _githubRepoName;
    private string _githubRepoOwner;
    private int _githubDiscussionNumber;
    private readonly int _numSecondsForNewComments;

    public ReplyToNewGithubDiscussion(ILogger log, string apimServiceName, string subscriptionKey, string githubRepoName, string githubRepoOwner, int githubDiscussionNumber, int numSecondsForNewComments) {
      _log = log;
      _apimService = new ApimService($"https://{apimServiceName}.azure-api.net", $"{subscriptionKey}");
      _githubRepoName = githubRepoName;
      _githubRepoOwner = githubRepoOwner;
      _githubDiscussionNumber = githubDiscussionNumber;
      _numSecondsForNewComments = numSecondsForNewComments;
    }

    public async Task RunAsync() {
      (string githubDiscussionId, GithubDiscussionComment[] comments) = await _apimService.ListDiscussionCommentsAsync(_githubRepoName, _githubRepoOwner, _githubDiscussionNumber);
      var newComments = comments.Where(c => c.CreatedAt > DateTimeOffset.UtcNow.AddSeconds(-_numSecondsForNewComments)).ToArray();
      _log.LogInformation($"Found {newComments.Count()} new comments");
      foreach (var comment in newComments) {
        var body = $"[Automated Reply using [APIM Authorizations](https://github.com/Azure/APIManagement-Authorizations)🗝️] Thank you for giving feedback, {comment.Author.Login}!";
        var result = await _apimService.ReplyToDiscussionCommentAsync(githubDiscussionId, comment.Id, body);
      }
    }
  }
}