# AuthorizationProvider Setup Guide


## General Setup Guide

For the instruction on a specific identity provider, see [the Setup Guide for a Specific IdentityProvider](#setup-guide-for-a-specific-identityprovider) below. 

### Create an OAuth App

In order to take advantage of Token Store against an external service (e.g. Dropbox), you first need to create a developer account with them. It will have a direct relationship with you as the app developer and can notify you of issues or unusual activity. They can also enforce security and usability limits on a per-application basis. 

Register an OAuth app with the identity provider (e.g. Dropbox) you want to call on behalf of your users. This is typically done through the service's developer site. On a high level, the steps generally look like this:

1. Go to the service's **Developer site** e.g. [Dropbox developer site](https://www.dropbox.com/developers/apps).
2. Create an account with the service using your contact information.
3. Create an app registration, and record the **App key** and **App secret** to be used when creating `AuthorizationProvider`
4. Set the redirect URI to the redirect URL given when creating `AuthorizationProvider` e.g. `https://global-test.consent.azure-apim.net/redirect`.

### Create a AuthorizationProvider
> The recommended way is to use the VSCode Extension (see [Quickstart guide](/docs/quickstart.md)) which will automatically prompt user for required fields depending on the chosen `identityProvider`.

`AuthorizationProvider` represents which OAuth app you are going to use. To create one, follow this step:

1. Choose identityProvider among [the list of available identityProviders](#list-of-available-identityproviders).
2. Provide clientId, clientSecret, scopes
3. Provide the value of the additional parameters (e.g. resourceUri) as `parameters` property. Parameters with default values do not need to be provided. 

Most identity provider doesn't require `parameters` so the request body would typically look like
```json
{
    "properties": {
        "identityProvider": "dropbox",
        "oauthsettings": {
            "clientId": "[your-aad-application-id]",
            "clientSecret": "[your-aad-application-key]",
            "scopes": "User.Read.All Group.ReadWrite.All Group.Read.All",
        }
    }
}
```

Some identity providers like `Azure Active Directory` have additional fields; for example, its request body would look like
```json
{
    "properties": {
        "identityProvider": "aad",
        "oauthsettings": {
            "clientId": "[your-aad-application-id]",
            "clientSecret": "[your-aad-application-key]",
            "scopes": "User.Read.All Group.ReadWrite.All Group.Read.All",
            "parameters": {
                "resourceUri": "https://graph.microsoft.com"
            }
        }
    }
}
```

## Setup Guide for a Specific IdentityProvider

### Azure Active Directory

1. Go to [Azure Portal application registration](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredApps).
2. Create a new application, Use a unique name for your app, select **Application type** *Web app / API*, and set sign-on URL to `https://global-test.consent.azure-apim.net/redirect`.
3. After creation record the **Application ID** values for future use.
4. Select **Settings**, then **Keys**, enter a *Key description*, select **Expires** value *Never*, then *Save*.
5. Record the Key Password Value (or **Application secret**) for future use. Note: This is the only time you will be able to retrieve the secret from the Azure portal UI.
6. Go to **Properties** and set the application property **Multi-tenanted** to *Yes*.
7. Go to **Required permissions**, click *Add*, then select the API you want to access from Microsoft Graph.
    1. To access *SharePoint* you can select **Office 365 SharePoint Online**, then check the appropriate permissions needed.
    2. To access *Microsoft Graph* itself you can select **Microsoft Graph**, then check the appropriate permissions needed.
    3. To access *Outlook* itself you can select **Office 365 Exchange Online**, then check the appropriate permissions needed.  
8. Under **Required permissions** click on **Grant permissions** as a tenant admin for permissions that need admin input.
9. Proceed to create AuthorizationProvider. Provide the value for the following parameter. (Note: In VSCode extension, users will specify only the `ResourceUri` parameter. Default value will be used for the rest.) 
   1. `ResourceUri`: The resource to get authorization for e.g. `https://graph.microsoft.com`
   2. (Optional) `GrantType`: Oauth2 grant type to use. Default is `Code`
   3. (Optional) `LoginUri`: The Azure Active Directory login URL. Default is `https://login.windows.net`
   4. (Optional) `TenantId`: The tenant ID of your AAD application. Default is `common`
   5. (Optional) `EnableOnbehalfOfLogin`: Enable the on-behalf-of login flow. Default is `false`
   
Here is the example request body:

```json
{
    "properties": {
        "identityProvider": "aad",
        "oauthsettings": {
            "clientId": "[your-aad-client-id]",
            "clientSecret": "[your-aad-client-secret]",
            "scopes": "[scope]",
            "parameters": {
                "resourceUri": "https://graph.microsoft.com"
                // ...use the default value for rest of the parameters
            }
        }
    }
}
```

### Dropbox

The following instructions describe the service creation steps that may be different from the general recipe presented above.

1. Go to the [Dropbox developer portal](https://www.dropbox.com/developers).
1. **Sign in** using the link on top-right of the web site. **[Sign up](https://www.dropbox.com/register)** if you do not have an account already.
1. [Create a new app](https://www.dropbox.com/developers/apps/create), choose **Dropbox API**, **Full Dropbox** access, and create a unique name for your app.
1. Record the **App key** and **App secret** values for future use.
1. Add to **Redirect URIs** the value `https://global-test.consent.azure-apim.net/redirect`.
1. Proceed to create AuthorizationProvider.

### Generic OAuth 2.0

The OAuth 2.0 generic service can be used to create a customized service resource, and is meant for developers who deeply understand the OAuth 2.0 redirect process. Any external OAuth 2.0 service can be used, so the application registration depends on the specific external service. The following instructions describe the service creation steps that may be different from the general recipe presented above.

1. Create the OAuth App for your service. It should be similar to [the general steps above](#create-an-oauth-app)
1. Proceed to create AuthorizationProvider. The payload for DropBox for example will look something like below:
    1. Set `clientid` and `clientsecret` to the **Client ID** and **Client Secret** values you obtained from the external service.
    1. Set the `AuthorizationUrlTemplate` and `AuthorizationUrlQueryStringTemplate` to represent the URI Token Store will redirect to on the external service.
    1. Set the `TokenUrlTemplate` and `TokenBodyTemplate` to describe the URI and data Token Store uses to get the refresh token from the external service.
    1. Set the `RefreshUrlTemplate` and `RefreshBodyTemplate` to represent the URI and data Token Store uses to get a refreshed token from the external service.

```json
{
    "properties": {
        "identityProvider": "oauth2",
        "oauthsettings": {
            "clientId": "[your-dropbox-client-id]",
            "clientSecret": "[your-dropbox-client-secret]",
            "scopes": "[scope]",
            "parameters": {
                "AuthorizationUrlTemplate": "https://www.dropbox.com/oauth2/authorize",
                "AuthorizationUrlQueryStringTemplate": "?client_id={ClientId}&response_type=code&redirect_uri={RedirectUrl}&force_reapprove=true&state={State}",
                "TokenUrlTemplate": "https://api.dropbox.com/oauth2/token",
                "TokenBodyTemplate": "code={Code}&grant_type=authorization_code&redirect_uri={RedirectUrl}&client_id={ClientId}&client_secret={ClientSecret}",
                "RefreshUrlTemplate": "https://api.dropbox.com/oauth2/token",
                "RefreshBodyTemplate": "refresh_token={RefreshToken}&grant_type=refresh_token&client_id={ClientId}&client_secret={ClientSecret}"
            }
        }
    }
}
```

### Facebook

The following instructions describe the service creation steps that may be different from the general recipe presented above.

1. Go to the [Facebook developer portal](https://developers.facebook.com/apps).
2. **Sign in** using the link on top-right of the web site.
3. Create a new application, by choosing a unique name for your app.
4. Record the **App ID** and **App secret** values for future use.
5. Add the **Facebook Login** product on the first page, and under **Settings**, set **Valid OAuth Redirect URIs** to `https://global-test.consent.azure-apim.net/redirect`.
6. Set rest of the required fields like *Privacy policy URL*, then *Save changes*.
7. Proceed to create AuthorizationProvider.

### Instagram

The following instructions describe the service creation steps that may be different from the general recipe presented above.

1. Go to the [Instagram developer portal](https://www.instagram.com/developer).
1. **Sign in** using the link on top-right of the web site.
1. Create a new client, by choosing a unique name for your app.
1. Record the **Client ID** and **Client Secret** values for future use.
1. Under **Security** tab, set **Valid redirect URIs** to `https://global-test.consent.azure-apim.net/redirect`.
1. Set rest of the required fields like *Company name*, then *Save changes*.
1. Proceed to create AuthorizationProvider; make sure the parameter `scopes` is set to `basic`.