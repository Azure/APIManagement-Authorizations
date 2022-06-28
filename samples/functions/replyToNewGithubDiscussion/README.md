# API Management Authorization Samples - Reply to New Github Discussion Comments using Azure functions

This sample uses API Management Authorizations + Azure functions to post automated reply in Github. Azure function is used as a trigger an execution at a regular interval and the APIM is used to attach the access token when calling Github endpoints. 

For live demo, try making a comment in https://github.com/APIManagementAuthorizations/APIManagement-Authorizations/discussions/2 

## How to Run Locally

1. Download public schema of Github GraphQL endpoints [here](https://docs.github.com/en/graphql/overview/public-schema)
1. Create an API Management in https://portal.azure.com
1. Go to APIs > + Add API > GraphQL. Check "Upload schema" box and upload the schema downloaded in step1. 
1. Attach an authorization to the APIs following [this step](https://docs.microsoft.com/en-us/azure/api-management/authorizations-how-to). 
1. Run [ReplyToNewGithubDiscussionTimeTrigger](./ReplyToNewGithubDiscussionTimeTrigger.cs) Azure Functions locally using your preferred methods. Detailed instructions on each method can be found in [this doc](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-local). Note that these environment variables need to be set: `APIM_SERVICE_NAME`, `SUBSCRIPTION_KEY`, `GITHUB_REPO_NAME`, `GITHUB_REPO_OWNER`, `GITHUB_DISCUSSION_NUMBER`.