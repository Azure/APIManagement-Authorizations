using System;
using System.Linq;
using System.Threading.Tasks;

namespace APIMAuthorizationsDemo.Function
{
  public class ReplyToNewGithubDiscussion {

    private ApimService _apimService;
    private string _githubRepoName;
    private string _githubRepoOwner;
    private int _githubDiscussionNumber;

    public ReplyToNewGithubDiscussion() {
      var apimServiceName = System.Environment.GetEnvironmentVariable("APIM_SERVICE_NAME", EnvironmentVariableTarget.Process);
      var subscriptionKey = System.Environment.GetEnvironmentVariable("SUBSCRIPTION_KEY", EnvironmentVariableTarget.Process);
      _apimService = new ApimService($"https://{apimServiceName}.azure-api.net", $"{subscriptionKey}");

      _githubRepoName = System.Environment.GetEnvironmentVariable("GITHUB_REPO_NAME", EnvironmentVariableTarget.Process);
      _githubRepoOwner = System.Environment.GetEnvironmentVariable("GITHUB_REPO_OWNER", EnvironmentVariableTarget.Process);
      _githubDiscussionNumber = Int32.Parse(System.Environment.GetEnvironmentVariable("GITHUB_DISCUSSION_NUMBER", EnvironmentVariableTarget.Process));
    }

    public async Task RunAsync() {
      (string githubDiscussionId, GithubDiscussionComment[] comments) = await _apimService.ListDiscussionCommentsAsync(_githubRepoName, _githubRepoOwner, _githubDiscussionNumber);
      var newComments = comments.Where(c => c.CreatedAt > DateTimeOffset.UtcNow.AddSeconds(-62)).ToArray();
      foreach (var comment in newComments) {
        var body = $"[Automated Reply using [APIM Authorizations](https://github.com/Azure/APIManagement-Authorizations)🗝️] Thank you for giving feedback, {comment.Author.Login}!";
        var result = await _apimService.ReplyToDiscussionCommentAsync(githubDiscussionId, comment.Id, body);
      }
    }
  }
}