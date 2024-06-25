param name string
param location string
@allowed(['Free', 'Standard'])
param sku string

var skus = {
  Free: {
    capacity: 1
    name: 'Free_F1'
    tier: 'Free'
  }
  Standard: {
    capacity: 1
    name: 'Standard_S1'
    tier: 'Standard'
  }
}

resource azWebPubSub 'Microsoft.SignalRService/webPubSub@2023-02-01' = {
  name: name
  location: location
  sku: skus[sku]
  identity: {
    type: 'None'
  }
  properties: {
    publicNetworkAccess: 'Enabled'
  }
}

output id string = azWebPubSub.id
output name string = azWebPubSub.name
