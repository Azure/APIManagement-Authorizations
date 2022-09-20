# Quickstart

## Prerequisites
The prerequisites for trying this feature are 

* Create an API Management instance in the West Central US region with the Developer SKU. (Basic, Standard, and Premium SKUs are also supported. Consumption SKU is not supported in the private preview.)
* Request that the API Management instance is enabled for the Token store. Read [how to request the Token store feature](https://github.com/Azure/APIManagement-TokenStore#prerequisite).
* Install the VS Code Extension for API Management.See the below steps for installing the VS Code Extension. (In the private preview there is no Azure portal experience.)

<br>

Download extension .vsix from [here](/extension).<br> 
Install the VS Code Extension following these [short instructions to install an extension from a VSIX](https://code.visualstudio.com/docs/editor/extension-marketplace#_install-from-a-vsix). <br>
Make sure to disable auto update extensions.

![Disable Extension Auto Update](/docs/images/Extension%20Disable%20Auto%20Update.jpg) 

<br>
<br>

## Overview 

This quickstart will show the basic functionality of the APIM Token store. We will use the APIM instance that was recently created in the West Central US region and create an API with three operations that communicates with the GitHub API. The GitHub API requires a Bearer token that is used for authorization, we will configure the APIM Token store to fetch the token and add it as a header to our http calls.   

We will create an API with three operations. 
* Getuser - the operation will return information from the GitHub API about the user.
* Getrepos - the operation will return an array of repos for the user from the GitHub API.
* Gettoken - the operation will return an accesstoken without calling the GitHub API. This token could be used to call the GitHub API. 

![Flow](/docs/images/gh_diagram_1.gif) <br>

1. Client calls API in APIM. 
2. A policy fetches a token from the Token store. 
3. The token is added to the backend call. 

![Flow](/docs/images/gh_diagram_2.gif) <br>

1. Client calls API in APIM. 
2. A policy fetches a token from the Token store. 
3. The token is returned back to the client. 

<br>

This quickstart has the following steps:

1. Enable managed identity for your APIM instance. 
2. Create an API with three operations and a managed identity using Bicep.
3. Configure an application in GitHub. 
4. Configure an AuthorizationProvider using VS Code for the GitHub api.
5. Add policies to the existing API to fetch tokens from the Token store. 
6. Validate that all three operations work.            

After you have gone through this quickstart you will have the basic understanding of the Token store feature, then you can proceed trying out the other AuthorizationProviders.  

---


## Step 1 Enable managed identity for your APIM instance

1. Go to the Azure portal and your APIM instance, in the menu choose "Managed identities", toggle the "Status" to "On", click "Save". Click "Yes" on the popup that says "Enable system assigned managed identity".

---

## Step 2 Create an API using Bicep

 
1. Verify that you are logged into the right subscription by running the following command in WSL, Cloudshell, Windows Terminal or other shell of your choice.  

```
az account show 
```
2. Create a file called main.bicep, copy the content below and change [YOUR APIM INSTANCE NAME] to your APIM instance name.

```
// APIM instance name 
var apim_name = '[YOUR APIM INSTANCE NAME]'


// Reference to APIM instance
resource apim 'Microsoft.ApiManagement/service@2021-04-01-preview' existing = {
  name: apim_name
}

// Api, tokenstoredemo-gh
resource api_tokenstoredemo_gh 'Microsoft.ApiManagement/service/apis@2021-04-01-preview' = {
  name: 'tokenstoredemo-gh'
  parent: apim
  properties: {
    displayName: 'tokenstoredemo-gh'
    path: '/tokenstoredemo-gh'
    serviceUrl: 'https://api.github.com/'
    protocols:[
      'https'
    ]
  }
}

// Operation, getrepos
resource operation_tokenstoredemo_gh_getrepos 'Microsoft.ApiManagement/service/apis/operations@2021-04-01-preview' = {
  name: 'getrepos'
  parent: api_tokenstoredemo_gh
  properties:{
    displayName: 'getrepos'
    urlTemplate: '/getrepos/{username}'
    method: 'GET'
    templateParameters: [
      {
        type: 'string'
        name: 'username'
      }
    ]
  }
}

// Operation, getuser 
resource operation_tokenstoredemo_gh_getuser 'Microsoft.ApiManagement/service/apis/operations@2021-04-01-preview' = {
  name: 'getuser'
  parent: api_tokenstoredemo_gh
  properties:{
    displayName: 'getuser'
    urlTemplate: '/getuser/{username}'
    method: 'GET'
    templateParameters: [
      {
        type: 'string'
        name: 'username'
      }
    ]
  }
}

// Operation, gettoken
resource operation_tokenstoredemo_gh_gettoken 'Microsoft.ApiManagement/service/apis/operations@2021-04-01-preview' = {
  name: 'gettoken'
  parent: api_tokenstoredemo_gh
  properties:{
    displayName: 'gettoken'
    urlTemplate: '/gettoken'
    method: 'GET'
  }
}

// Policy for getrepos operation
resource policy_operation_tokenstoredemo_gh_getrepos 'Microsoft.ApiManagement/service/apis/operations/policies@2021-01-01-preview' = {
  parent: operation_tokenstoredemo_gh_getrepos
  name: 'policy'
  properties: {
    value: tokenstoredemo_gh_getrepos_xml
    format: 'rawxml'
  }
}

// Policy for getuser operation
resource policy_operation_tokenstoredemo_gh_getuser 'Microsoft.ApiManagement/service/apis/operations/policies@2021-01-01-preview' = {
  parent: operation_tokenstoredemo_gh_getuser
  name: 'policy'
  properties: {
    value: tokenstoredemo_gh_getuser_xml
    format: 'rawxml'
  }
}

// Policy variable for getrepos
var tokenstoredemo_gh_getrepos_xml = '''
<policies>
    <inbound>
        <rewrite-uri template="/users/{username}/repos" />
        <base />
    </inbound>
    <backend>
        <base />
    </backend>
    <outbound>
        <base />
    </outbound>
    <on-error>
        <base />
    </on-error>
</policies>
'''

// Policy variable for getuser
var tokenstoredemo_gh_getuser_xml = '''
<policies>
    <inbound>
        <rewrite-uri template="/users/{username}" />
        <base />
    </inbound>
    <backend>
        <base />
    </backend>
    <outbound>
        <base />
    </outbound>
    <on-error>
        <base />
    </on-error>
</policies>
'''
```

3. Change [YOUR RESOURCEGROUP] to the same resourcegroup as your APIM instance and deploy main.bicep by running the following command. 

```
az deployment group create --resource-group [YOUR RESOURCEGROUP] --template-file main.bicep
```

4. Verify that the api "tokenstoredemo-gh" are created in your APIM instance. 

---


## Step 3 Configure a new application in GitHub

1. Log in to GitHub at https://github.com/settings/developers.
2. Click on the button "New OAuth App". 
3. Enter 
    - `Applicationname` : Give the application a name.
    - `Homepage URL` : Not relevant, enter any http address. 
    - `Authorization callback URL` : https://global-test.consent.azure-apim.net/redirect

![Register application](/docs/images/gh_register_app.gif)

4. Click "Register application", when the application is registered, open it. 
5. Copy the Client ID, it will be used later. 
6. Generate a new secret and copy it, will be used later.   

![Generate secret](/docs/images/gh_add_secret.gif)

---

## Step 4 Configure AuthorizationProvider, Authorization and Permission for GitHub 

1. Open VS Code and go to the Azure extension, find your APIM instance under "API MANAGEMENT" section.
2. Right click on "AuthorizationProviders" and choose "Create Authorization Provider".
3. Configure the AuthorizationProvider
    - `Name` : GitHub
    - `Identityprovider` : Choose GitHub
    - `ClientId` : Use the Client ID from GitHub 
    - `Client Secret` : Use the Client Secret from GitHub
    - `Scope` : Leave blank, hit enter  

![AuthorizationProvider](/docs/images/gh_configure_ap.gif)

<br>

4. Under "AuthorizationProviders" the "GitHub" AuthorizationProvider has been created. Drill down to "Authorizations" and right click, choose "Create Authorization". 
5. Enter "auth" as the name. 
6. A browser opens and you need to authorize. 
7. A redirect will occur and a confirmation is required. Click "Allow access" 
8. An "Authorization" called "auth" is created and next to it the status is "error" (a known bug). Right click on "Authorizations" and choose "Refresh" and you will see the status has been changed to "Connected". If you drill down under your newly created authorization you will see your email and your AAD objectID under permissions. These are the objectID's that has permission to use the above authorization. Here we need to add the APIM instance managed identity. 

<br>

![Authorization](/docs/images/gh_configure_auth.gif)

<br>

9. Right click on "Permissions", choose "Create Permission". In the dropdown choose the managed identity for your APIM instance. The permission is being created and you can see the managed identity listed under "Permissions"

<br>

![Permission](/docs/images/gh_configure_perm.gif)

---

## Step 5 Add policies to the existing API

Now it's time to configure your API/operation with a policy that fetches the access token to be used to call the backend GitHub API. 

1. In VS Code Extension right click on the "Authorization" called "auth" that you recently created and choose "Copy Policy Snippet". 
Here you have two options: 
    - `"Get the access token back"` - In this case the token that is being fetched will be returned and no call to the backend API will be done. Use this option if you just want to get the token back from the Token store.
    - `"Attach the access token to the backend call"` - In this case the token is being fetched and an Authorization header is being added to the backend call. <br>Choose `"Attach the access token to the backend call"`. 

2. The next step is to choose which identity that will be used to fetch the token from the Token store. 
    - `"managed"` - In this case the managed identity of the APIM instance will be used. 
    - `"jwt"` - In this case you need to call the APIM API with and Authorization header that consists of a bearer token for the identity to be used. You can get your own token by running "az account get-access-token".<br>
    Choose `"managed"`. 

3. A policy snippet that is shown below has now been copied to the clipboard. 

```

<!-- Add to the inbound policy -->
<get-authorization-context 
    provider-id="github"
    authorization-id="auth" 
    context-variable-name="github-auth-context" 
    ignore-error="false" 
    identity-type="managed" />
<set-header name="Authorization" exists-action="override">
    <value>@("Bearer " + ((Authorization)context.Variables.GetValueOrDefault("github-auth-context"))?.AccessToken)</value>
</set-header>

```

![Apipolicy](/docs/images/gh_policy_managed.gif)

<br>

4. Go to APIs and navigate to "tokenstoredemo-gh" --> Operations --> "getrepos" --> Policy

5. Paste in the policy snippet from the clipboard in the inbound section under "< base />", then do the same for the "getuser" operation. Use shortcut "Ctrl + S" to upload the changes to APIM or choose File --> Save in the menu. 

<br>

![Apipolicy](/docs/images/gh_api_policy_1.gif)

<br>

6. For the "gettoken" operation we need another policy snippet so we navigate to our "Authorization" called "auth" and right click --> Copy Policy Snippet --> Get the access token back --> managed. A policy snippet that is shown below has now been copied to the clipboard.

```
<!-- Add to the inbound policy -->
<get-authorization-context 
    provider-id="github" 
    authorization-id="auth" 
    context-variable-name="github-auth-context" 
    ignore-error="false" 
    identity-type="managed" />
<return-response>
    <set-status code="200" />
    <set-body template="none">@(((Authorization)context.Variables.GetValueOrDefault("github-auth-context"))?.AccessToken)</set-body>
</return-response>

```

7. Go to APIs and navigate to "tokenstoredemo-gh" --> Operations --> "gettoken" --> Policy

8. Paste in the policy snippet from the clipboard in the inbound section under "< base />". Use shortcut "Ctrl + S" to upload the changes to APIM or choose File --> Save in the menu.

<br>

![Apipolicy](/docs/images/gh_api_policy_2.gif)

---

## Step 6 Validate that all three operation works  :-)   

Use the Azure portal, Postman, REST Client (VS Code extension) or your choice of tool to test your operations.
If you are using the Azure portal remember to check the "Bypass CORS proxy", this is needed because GitHub requires "User-Agent" as a header. For more information on this requirement please [read this](https://docs.github.com/en/rest/overview/resources-in-the-rest-api#user-agent-required).  

```
// getrepos
// This operation should return a list of repos for the GitHub username
https://[YOUR APIM INSTANCE].azure-api.net/tokenstoredemo-gh/getrepos/[YOUR GITHUB USERNAME]
Ocp-Apim-Subscription-Key: [YOUR SUBSCRIPTION KEY]

// getuser
// This operation should return information of the GitHub user
https://[YOUR APIM INSTANCE].azure-api.net/tokenstoredemo-gh/getuser/[YOUR GITHUB USERNAME]
Ocp-Apim-Subscription-Key: [YOUR SUBSCRIPTION KEY]

// gettoken
// This operation should return a token to be used with the GitHub API
https://[YOUR APIM INSTANCE].azure-api.net/tokenstoredemo-gh/gettoken
Ocp-Apim-Subscription-Key: [YOUR SUBSCRIPTION KEY]
```
