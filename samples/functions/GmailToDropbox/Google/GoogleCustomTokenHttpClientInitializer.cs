using Google.Apis.Http;

namespace TokenStoreDemo.Function
{
    /// <summary>
    /// Custom ConfigurationHttpClientInitializer that provides the bearer token on initialization, 
    /// instead of using the default BaseClientService with GoogleWebAuthorizationBroker which requires 
    /// App Key and App Secret
    /// </summary>
    public class GoogleCustomTokenHttpClientInitializer : IConfigurableHttpClientInitializer
    {
        private readonly string _bearerToken;

        public GoogleCustomTokenHttpClientInitializer(string bearerToken)
        {
            _bearerToken = bearerToken;
        }

        public void Initialize(ConfigurableHttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");
        }
    }
}