targetScope = 'subscription'

param location string
param environment string
param resourceGroupName string
@allowed(['Free', 'Standard'])
param sku string
param functionAppName string
param eventHandlerAddress string

var name = 'ws-sweepersmackdown-${environment}'
var hubName = 'Game'

resource azResourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' existing = {
  name: resourceGroupName
}

module azWebPubSub '../resources/azWebPubSub.bicep' = {
  name: name
  scope: azResourceGroup
  params: {
    name: name
    location: location
    sku: sku
  }
}

module azWebPubSubHub '../resources/azWebPubSubHub.bicep' = {
  name: hubName
  scope: azResourceGroup
  params: {
    name: hubName
    webPubSubName: azWebPubSub.outputs.name
    functionAppName: functionAppName
    eventHandlerAddress: eventHandlerAddress
  }
}

output id string = azWebPubSub.outputs.id
output name string = azWebPubSub.outputs.name
output hubId string = azWebPubSubHub.outputs.id
output hubName string = azWebPubSubHub.outputs.name
