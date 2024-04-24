// Parameters
param webPubsubName string
param eventHandlerAddress string

// Get web pubsub
resource azWebPubsub 'Microsoft.SignalRService/webPubSub@2023-02-01' existing = {
  name: webPubsubName
}

// Create web pubsub hub
resource azWebPubsubHub 'Microsoft.SignalRService/webPubSub/hubs@2023-02-01' = {
  name: 'Game'
  parent: azWebPubsub
  properties: {
    anonymousConnectPolicy: 'allow'
    eventHandlers: [
      {
        systemEvents: [
          'connect'
          'connected'
          'disconnected'
        ]
        urlTemplate: eventHandlerAddress
        userEventPattern: '*'
      }
    ]
  }
}

// Outputs
output name string = azWebPubsubHub.name
