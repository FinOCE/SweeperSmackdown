// Parameters
param name string
param location string

// Create web pubsub
resource azWebPubsub 'Microsoft.SignalRService/webPubSub@2023-02-01' = {
  name: name
  location: location
  sku: {
    capacity: 1
    name: 'Standard_S1'
    tier: 'Standard'
  }
  identity: {
    type: 'None'
  }
  properties: {
    publicNetworkAccess: 'Enabled'
  }
}

// Outputs
output name string = azWebPubsub.name
