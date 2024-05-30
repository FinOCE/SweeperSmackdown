// Parameters
param name string
param location string

// Create web pubsub
resource azWebPubsub 'Microsoft.SignalRService/webPubSub@2023-02-01' = {
  name: name
  location: location
  sku: {
    capacity: 1
    name: 'Free_F1' // 'Standard_S1'
    tier: 'Free' // 'Standard'
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
