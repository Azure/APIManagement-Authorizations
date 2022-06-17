
# Azure API Management Authorizations (preview) :rocket: 

Introducing **[API Management Authorizations](https://docs.microsoft.com/en-gb/azure/api-management/authorizations-overview)** 🗝️  - *Simple, Distributed, Secure, Managed Token Store*

<p align="center">
  <img src="overview.png" />
</p>


### Overview 🎁
API Management Authorizations greatly simplifies the process of authenticating and authorizing user's across one (or) more SaaS services 👍. It reduces the development cost in ramping up, implementing and maintaining security features with service integrations 💰.

Configure OAuth, Consent, Acquire Tokens, Cache Tokens and Refresh Tokens for multiple-services without writing a single line of code. Let API Management do all the heavy lifting for you, while you focus on the application/domain logic.

To get more information, please visit the official Azure documentation [here](https://docs.microsoft.com/en-gb/azure/api-management/authorizations-overview).   

### Scenarios 🧰

Here are some example scenarios where this feature could be used:
- Easily connect to SaaS backend by attaching the stored Authorization Token and proxying requests. 
- Proxy requests to a Azure AppService WebApp (or) Azure Functions backend by attaching the Authorization Token that can later send requests to SaaS backend applying transformation logic. 
- Proxy requests to GraphQL federation backends by attaching multiple access tokens to easy perform federation.
- Expose a retrieve token endpoint, acquire cached token and call the SaaS on-behalf of user from any compute, say Console App or Kubernetes Demon. Combine your fav. SaaS Sdk in a supported language.
- Azure Functions unattended scenarios is a breeze while connecting to multiple SaaS.
- Durable Functions gets a step closer to Logic Apps with SaaS connectivity.
- Every API in API Management can act as a Logic Apps Custom Connector.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.


## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.