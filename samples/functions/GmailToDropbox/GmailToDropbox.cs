using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Gmail.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace TokenStoreDemo.Function
{
    public class GmailToDropbox
    {
        private readonly string[] AllowedTokenStoreAudience = new string[] { "https://management.core.windows.net/", "https://apihub.azure.com" };

        /// <summary>
        /// This function (1) gets the latest attachment in your gmail inbox and (2) upload it to your dropbox folder. 
        /// The former utilizes the "Get Token Back" pattern, where API Management Token store is used to store tokens
        /// only. The latter utilizes the "Attach Token to Backend Call" pattern, where API Management is working as a 
        /// proxy that attaches the access token to the backend call using the policy. 
        /// 
        /// Your APIM Service needs to have setup two endpoints:
        /// - token-store/fetch: Endpoint to fetch tokens given the authorizationProviderId and authorizationId
        /// - dropbox/files/upload: Proxy Endpoint that attaches your dropbox authorization before calling dropbox. 
        /// </summary>
        [FunctionName("GmailToDropbox")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                // Set environment variables in Azure Functions Configuration and local.settings.json for (local development)
                var myApimEndpoint = Environment.GetEnvironmentVariable("MyApimEndpoint") ?? throw new Exception("MyApimEndpoint Environment variable not set");
                var mySubscriptionKey = Environment.GetEnvironmentVariable("MyApimSubscriptionKey") ?? throw new Exception("MyApimSubscriptionKey Environment variable not set");
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var managedIdentityToken = await azureServiceTokenProvider.GetAccessTokenAsync(AllowedTokenStoreAudience.First());
                var apimService = new ApimService(myApimEndpoint, mySubscriptionKey, managedIdentityToken);

                // 1. Get the Latest Attachment from Gmail by utilizing "GetTokenBack" endpoint of my API Management
                var attachment = await GetLatestGmailAttachment(apimService);
                var message = "No attachment found";
                if (attachment != null)
                {
                    // 2. Upload to Dropbox by utilizing the "Dropbox - UploadFile" endpoint of my API Management
                    var uploadedFileName = await UploadToDropbox(apimService, attachment.Item1, attachment.Item2);
                    message = $"Uploaded the following files to dropbox: {uploadedFileName}";
                }

                log.LogInformation(message);
                return new OkObjectResult(message);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                throw;
            }
        }

        private async Task<Tuple<string, string>> GetLatestGmailAttachment(ApimService apimService)
        {
            var gmailToken = await apimService.GetTokenBackAsync("google1", "auth1");
            var gmailService = new GmailService(
                new Google.Apis.Services.BaseClientService.Initializer()
                {
                    HttpClientInitializer = new GoogleCustomTokenHttpClientInitializer(gmailToken)
                });

            var messages = await gmailService.Users.Messages.List("me").ExecuteAsync();
            foreach (var message in messages.Messages)
            {
                var fullMessage = await gmailService.Users.Messages.Get("me", message.Id).ExecuteAsync();
                var attachmentParts = fullMessage.Payload?.Parts?.Where(p => p?.Body?.AttachmentId != null);
                if (attachmentParts == null)
                {
                    continue;
                }

                foreach (var attachmentPart in attachmentParts)
                {
                    var attachment = await gmailService.Users.Messages.Attachments.Get("me", message.Id, attachmentPart.Body.AttachmentId).ExecuteAsync();
                    return new Tuple<string, string>(attachmentPart.Filename, attachment.Data);
                }
            }
            return null;
        }

        private async Task<string> UploadToDropbox(ApimService apimService, string attachmentFileName, string attachmentData)
        {
            var attachmentRawContent = Encoding.Default.GetString(Base64UrlTextEncoder.Decode(attachmentData));
            var filename = $"{DateTime.UtcNow:s}-{attachmentFileName}";
            var result = await apimService.DropboxUploadFileAsync(filename, attachmentRawContent);
            return result["name"].ToString();
        }
    }
}
