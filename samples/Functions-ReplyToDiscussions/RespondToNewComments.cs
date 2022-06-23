using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyAmbassadorDemo.Function
{
  public class RespondToNewComments {
    private const string APIM_SERVICE_NAME = "{APIM_SERVICE_NAME}";
    private const string SUBSCRIPTION_KEY = "{SUBSCRIPTION_KEY}";
    private const string DISCUSSION_ID = "D_kwDOHiNKns4AP3wj";

    private ApimService _apimService;

    public RespondToNewComments() {
      _apimService = new ApimService($"https://{APIM_SERVICE_NAME}.azure-api.net", $"{SUBSCRIPTION_KEY}");
    }

    public async Task RunAsync() {
      var comments = await _apimService.ListDiscussionCommentsAsync();
      var newComments = comments.Where(c => c.CreatedAt > DateTimeOffset.UtcNow.AddSeconds(-12)).ToArray();
      foreach (var comment in newComments) {
        var body = $"[Automated Reply using [APIM Authorizations](https://github.com/Azure/APIManagement-Authorizations)🗝️] Thank you for giving feedback, {comment.Author.Login}!";
        var result = await _apimService.ReplyToDiscussionCommentAsync(DISCUSSION_ID, comment.Id, body);
      }
    }
  }
}