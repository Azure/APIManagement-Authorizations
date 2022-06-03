param apim_name string
param location string = resourceGroup().location 


// APIM instance
resource apim 'Microsoft.ApiManagement/service@2021-12-01-preview' = {
  name: apim_name
  location: location
  sku: {
    name: 'Developer'
    capacity: 1
  }
  properties: {
    publisherName: 'John Doe'
    publisherEmail: 'john.doe@nomail.com'
  }
  identity: {
    type: 'SystemAssigned'
  }
}
