using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMAuthorizationsDemo.Function
{
    internal class ApimService
    {
        private readonly string _baseUrl;
        private readonly string _subscriptionKey;
        private readonly HttpClient _httpClient;
        private readonly GraphQLHttpClient _graphQLClientForGithubEndpoints;

        public ApimService(string baseUrl, string subscriptionKey, string identityToken = null)
        {
            _baseUrl = baseUrl;
            _subscriptionKey = subscriptionKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            _graphQLClientForGithubEndpoints = new GraphQLHttpClient(_baseUrl + "/api/github-gql", new NewtonsoftJsonSerializer());
            _graphQLClientForGithubEndpoints.HttpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
        }

        public async Task<string> GetTokenBackAsync(string providerId, string authorizationId)
        {
            var endpoint = $"{_baseUrl}/tokenstore/fetch?authorizationProviderId={providerId}&authorizationId={authorizationId}";
            var response = await _httpClient.PostAsync(endpoint, null);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Fetch token call unsuccessful: {content}");
            }
            return content;
        }

        public async Task<GithubDiscussionComment[]> ListDiscussionCommentsAsync() 
        {
            var request = new GraphQLRequest() {
                Query = @"
query {
	repository(name: ""APIM-Ambassador-demo"", owner: ""APIManagementAuthorizations"") {
		discussion(number: 1) {
			comments(last: 100) {
				nodes {
					author {
						login
					}
					body
					createdAt
					id
					url
				}
			}
		}
	}
}
"
            };
            var response = await _graphQLClientForGithubEndpoints.SendQueryAsync<JObject>(request);
            var comments = response.Data["repository"]["discussion"]["comments"]["nodes"].ToObject<GithubDiscussionComment[]>();
            return comments;
        }

        public async Task<bool> ReplyToDiscussionCommentAsync(
            string discussionId, string replyToId, string body) 
        {
            var query = $"mutation {{ addDiscussionComment(input: {{ body: \"{body}\", discussionId: \"{discussionId}\", replyToId: \"{replyToId}\" }}) {{comment {{ url }} }} }}";
            var request = new GraphQLRequest() {
                Query = query
            };
            var response = await _graphQLClientForGithubEndpoints.SendQueryAsync<JObject>(request);
            return true;
        }
    }
}