# Documentation

## Resources

| Resource name | Parent resource | Description | Supported actions |
|---------------|---|--|---|
| IdentityProvider (aka ServiceProvider) | APIM Service | Metadata about the identity providers used by the AuthorizationProvider. | List |
| AuthorizationProvider | APIM Service| represents which OAuth app you are going to use. See [this Setup Guide](/docs/authorizationprovider-setup-guide.md) for detail. | List, Read, CreateOrUpdate, Delete |
| Authorization | AuthorizationProvider | represents which credential you are going to use. One Authorization emits one access token | List, Read, CreateOrUpdate, Delete, GetLoginLinks |
| Permission | Authorization | represents which AAD identity has access to the `Authorization`. | List, Read, CreateOrUpdate, Delete |

See [this Postman collection](https://www.getpostman.com/collections/a24f9cb922e6adde8c5e) for a sample of all supported operations of these resource.

---

## Get-Authorization-Context Policy
> Note that you can utilize [Copy Policy Snippet](/docs/quickstart.md#step-4-copy-get-authorization-context-policy-snippet-and-add-to-your-api-management-policies) command on VSCode extension to easily generate the policy snippet for your purpose.

To access the authorization and its access token at runtime, you need to add `get-authorization-context` to the API you want to use. This policy gets the authorization context of the specified authorization, including the access token. 

### Policy Statement
```xml
<get-authorization-context
    provider-id="authorization provider id" 
    authorization-id="authorization id" 
    context-variable-name="variable name" 
    identity-type ="managed | JWT"
    identity = "JWT bearer token"
    ignore-error="true | false" />
```

### Example 1: Attach token to the backend call
```xml
<get-authorization-context 
    provider-id="google1" 
    authorization-id="conn1" 
    context-variable-name="my-variable" 
    identity-type="managed" 
    ignore-error="false" />
<set-header name="Authorization" exists-action="override">
    <value>@("Bearer " + ((Authorization)context.Variables.GetValueOrDefault("my-variable"))?.AccessToken)</value>
</set-header>
```

### Example 2: Get token back using the incoming bearer token
```xml
<get-authorization-context 
    provider-id="google1" 
    authorization-id="conn1" 
    context-variable-name="my-variable" 
    identity-type="jwt" 
    identity="@(context.Request.Headers["Authorization"][0].Replace("Bearer ", ""))"
    ignore-error="false" />
<return-response>
    <set-status code="200" />
    <set-body template="none">@(((Authorization)context.Variables.GetValueOrDefault("my-context"))?.AccessToken)</set-body>
</return-response>
```

### Attributes
| Name | Description | Required | Default |
|---|---|---|---|
| `provider-id` | The authorization provider resource identifier. | Yes |   |
| `authorization-id` | The authorization resource identifier. | Yes |   |
| `context-variable-name` | The name of the context variable to receive the [`Authorization` object](#authorization-object). | Yes |   |
| `identity-type` | Type of identity to be checked against the Authorization permissions. <br> - `managed`: managed identity of the APIM service will be used. <br> - `jwt`: Jwt bearer token specified in the `identity` attribute will be used. | No | managed |
| `identity` | An AAD JWT bearer token to be checked against the Authorization permissions. Ignored if identity-type != "jwt". <br><br>Expected Claims: <br> - audience: https://management.core.windows.net/ or https://apihub.azure.com <br> - oid: Permission object id <br> - tid: Permission tenant id | No |   |
| `ignore-error` | If acquiring the authorization context results in an error (e.g. the authorization resource is not found or is in an error state): <br> - ignore-error="true": the context variable is assigned a value of null. <br> - ignore-error="false": return 500 | No | false |


### Authorization Object

The Authorization context variable receives an object of type `Authorization`.

```c#
class Authorization
{
    public string AccessToken { get; }
    public IReadOnlyDictionary<string, object> Claims { get; }
}
```

| Property Name | Description |
| -- | -- |
| AccessToken | Bearer access token to authorize a backend HTTP request. | 
| Claims | Claims returned from the authorization server’s token response API (see [RFC6749#section-5.1](https://datatracker.ietf.org/doc/html/rfc6749#section-5.1)) |