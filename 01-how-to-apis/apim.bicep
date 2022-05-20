param apim_name string
param location string = resourceGroup().location 


// APIM instance
resource apim 'Microsoft.ApiManagement/service@2021-08-01' = {
  name: apim_name
  location: location
  sku: {
    name: 'Consumption'
    capacity: 0
  }
  properties: {
    publisherName: 'John Doe'
    publisherEmail: 'john.doe@nomail.com'
  }
  identity: {
    type: 'SystemAssigned'
  }
}
