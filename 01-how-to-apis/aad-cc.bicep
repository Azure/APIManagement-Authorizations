param apim_name string 


resource apim 'Microsoft.ApiManagement/service@2021-08-01' existing = {
  name: apim_name
}

// *******************
// One api
// *******************

resource aad_api 'Microsoft.ApiManagement/service/apis@2021-08-01' = {
  name: 'aad-cc'
  parent: apim
  properties: {
    serviceUrl:'https://graph.microsoft.com'
    path: 'aad-cc'
    displayName:'aad-cc'
    protocols:[
      'https'
    ]
  }
}

// *******************
// Two operations
// *******************

resource aad_api_getdata 'Microsoft.ApiManagement/service/apis/operations@2021-08-01' = {
  name: 'getdata'
  parent: aad_api
  properties: {
    method: 'GET'
    urlTemplate: '/data'
    displayName: 'getdata'
  }
}

resource aad_api_gettoken 'Microsoft.ApiManagement/service/apis/operations@2021-08-01' = {
  name: 'gettoken'
  parent: aad_api
  properties: {
    method: 'GET'
    urlTemplate: '/token'
    displayName: 'gettoken'
  }
}

// *******************
// Three policies, one for api and two for each operation
// *******************

resource aad_api_policy 'Microsoft.ApiManagement/service/apis/policies@2021-08-01' = {
  parent: aad_api
  name: 'policy'
  properties: {
    value: api_policy
    format: 'xml'
    
  }
}

resource aad_getdata_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2021-08-01' = {
  parent: aad_api_getdata
  name: 'policy'
  properties: {
    value: operation_data_policy
    format: 'xml'
  }
}

resource aad_gettoken_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2021-08-01' = {
  parent: aad_api_gettoken
  name: 'policy'
  properties: {
    value: operation_token_policy
    format: 'xml'
  }
}
 

// *******************
// Three policies
// *******************
var api_policy = '''<policies>
<inbound>
    <base />
    <choose>
        <when condition="@(context.Request.Url.Query.GetValueOrDefault(&quot;idtype&quot;,&quot;managed&quot;).Equals(&quot;managed&quot;))">
            <get-authorization-context provider-id="@(context.Request.Url.Query.GetValueOrDefault(&quot;providerid&quot;,&quot;aad-02-cc&quot;))" authorization-id="@(context.Request.Url.Query.GetValueOrDefault(&quot;authid&quot;,&quot;auth-01&quot;))" context-variable-name="auth-context" ignore-error="false" identity-type="managed" />
        </when>
        <otherwise>
            <get-authorization-context provider-id="@(context.Request.Url.Query.GetValueOrDefault(&quot;providerid&quot;,&quot;aad-02-cc&quot;))" authorization-id="@(context.Request.Url.Query.GetValueOrDefault(&quot;authid&quot;,&quot;auth-01&quot;))" context-variable-name="auth-context" ignore-error="false" identity-type="jwt" identity="@(context.Request.Headers[&quot;Authorization&quot;][0].Replace(&quot;Bearer &quot;, &quot;&quot;))" />
        </otherwise>
    </choose>
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
</policies>'''


var operation_token_policy = '''<policies>
<inbound>
    <base />
    <return-response>
        <set-body>@(((Authorization)context.Variables.GetValueOrDefault(&quot;auth-context&quot;))?.AccessToken)</set-body>
    </return-response>
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
</policies>'''


var operation_data_policy = '''<policies>
<inbound>
    <base />
    <set-header name="Authorization" exists-action="override">
        <value>@("Bearer " + ((Authorization)context.Variables.GetValueOrDefault(&quot;auth-context&quot;))?.AccessToken)</value>
    </set-header>
    <set-variable name="uri" value="@{
      string oid = &quot;&quot;;
      string token = ((Authorization)context.Variables.GetValueOrDefault(&quot;auth-context&quot;))?.AccessToken;
      Jwt jwt;
      if (token.TryParseJwt(out jwt))
      {
          oid = jwt.Claims.GetValueOrDefault(&quot;oid&quot;, &quot;empty&quot;);
      }

      return &quot;/v1.0/users/&quot; + oid;
  }" />
  <rewrite-uri template="@((string)context.Variables[&quot;uri&quot;])" copy-unmatched-params="false" />

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
</policies>'''



