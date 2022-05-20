targetScope='subscription'

param apim_name string 
param rg_name string 
param location string 

resource resourcegroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: rg_name
  location: location
}

module apim 'apim.bicep' = {
  name: 'apim'
  scope: resourcegroup
  params: {
    apim_name: apim_name
  }
}


module google 'google.bicep' = {
  name: 'google'
  scope: resourcegroup
  dependsOn: [
    apim
  ]
  params: {
    apim_name: apim_name
  }
}

module dropbox 'dropbox.bicep' = {
  name: 'dropbox'
  scope: resourcegroup
  dependsOn: [
    apim
  ]
  params: {
    apim_name: apim_name
  }
}

module github 'github.bicep' = {
  name: 'github'
  scope: resourcegroup
  dependsOn: [
    apim
  ]
  params: {
    apim_name: apim_name
  }
}


module linkedin 'linkedin.bicep' = {
  name: 'linkedin'
  scope: resourcegroup
  dependsOn: [
    apim
  ]
  params: {
    apim_name: apim_name
  }
}

module spotify 'spotify.bicep' = {
  name: 'spotify'
  scope: resourcegroup
  dependsOn: [
    apim
  ]
  params: {
    apim_name: apim_name
  }
}

module aad 'aad.bicep' = {
  name: 'aad'
  scope: resourcegroup
  dependsOn: [
    apim
  ]
  params: {
    apim_name: apim_name
  }
}

module aad_cc 'aad-cc.bicep' = {
  name: 'aad-cc'
  scope: resourcegroup
  dependsOn: [
    apim
  ]
  params: {
    apim_name: apim_name
  }
}

module oauth2 'oauth2.bicep' = {
  name: 'oauth2'
  scope: resourcegroup
  dependsOn: [
    apim
  ]
  params: {
    apim_name: apim_name
  }
}
