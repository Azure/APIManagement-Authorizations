 # README

 This function (1) gets the latest attachment in your gmail inbox and (2) upload it to your dropbox folder. 
 The former utilizes the "Get Token Back" pattern, where API Management Token store is used to store tokens
 only. The latter utilizes the "Attach Token to Backend Call" pattern, where API Management is working as a 
 proxy that attaches the access token to the backend call using the policy. 

# Demo

See this [demo video](https://microsoft-my.sharepoint.com/:v:/p/seaki/EXurGJG_zL5DqplKalCcqVUB8N-S8u4FtUqKEASaV0DUtA?e=rfa8jk).

# How to Run

## Setup API Management
1. Setup the authorizations for your gmail and dropbox account following [quickstart guide](/docs/quickstart.md) or [demo video](/scenarios/Setup%20Token%20Store.mp4)
1. Create a corresponding permission for the identity making the call e.g. your account (local development) or manged identity of the functions app (Azure Functions)
2. Setup `token-store/fetch` endpoint
   1. Import from [this OpenAPI spec](/scenarios/functions/GmailToDropbox/Apim/token-store-open-api.json)
   2. Set `API URL Suffix` to `token-store`
   3. Copy and paste the policy from [this file](./Apim/token-store-policy.xml)
3. Setup `dropbox/files/upload`
   1. Import from [this OpenAPI spec](/scenarios/functions/GmailToDropbox/Apim/dropbox-open-api.json)
   2. Set `API URL Suffix` to `dropbox`
   3. Copy and paste the policy from [this file](./Apim/dropbox-policy.xml). Replace `MY_DROPBOX_PROVIDER_ID` and `MY_DROPBOX_AUTHORIZATION_ID` to the value from Step 1.

## Run Azure Functions Locally
1. In [local.settings.json](./local.settings.json), provide your API management endpoint and subscription key.
2. Open this folder in VSCode Extension and run the local function app through [Azure Functions Extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions).

## Run Azure Functions in the Cloud
> Make sure Permission for your Azure Functions' managed identity has been created.
1. Publish the app to your Azure Functions using [Azure Functions Extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions).
2. Navigate to your Azure Functions in Azure Portal. Go to Configuration and set `MyApimEndpoint` to your API Management endpoint and `MyApimSubscriptionKey` to your API Management subscription key.
3. Execute the function. 