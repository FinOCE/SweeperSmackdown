targetScope = 'subscription'

@description('The environment to build in')
@allowed(['beta', 'prod'])
param environment string
@description('The location to deploy to')
param location string

// IMPORTANT: vars must remain identical to `main.bicep`

var resourceGroup = 'rg-sweepersmackdown-${environment}'
var resourceToken = take(toLower(uniqueString(subscription().id, environment, location, 'api')), 7)
var webPubSubName = 'ws-api-${environment}-${resourceToken}'
var webPubSubHubName = 'Game'
var functionAppName = 'fa-api-${environment}-${resourceToken}'

resource azResourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' existing = {
  name: resourceGroup
}

module azWebPubSubHub './resources/azWebPubSubHub.bicep' = {
  name: webPubSubHubName
  scope: azResourceGroup
  params: {
    name: webPubSubHubName
    webPubSubName: webPubSubName
    functionAppName: functionAppName
  }
}
