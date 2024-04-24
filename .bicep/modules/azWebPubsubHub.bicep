// Parameters
param webPubsubName string
param eventHandlerAddress string
param functionAppName string

// Get web pubsub
resource azWebPubsub 'Microsoft.SignalRService/webPubSub@2023-02-01' existing = {
  name: webPubsubName
}

// Get function app's web pubsub extension system key
var extensionKey = listkeys('${resourceId('Microsoft.Web/sites', functionAppName)}/host/default/','2022-09-01').systemkeys.webpubsub_extension

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
        urlTemplate: '${eventHandlerAddress}?code=${extensionKey}'
        userEventPattern: '*'
      }
    ]
  }
}

// Outputs
output name string = azWebPubsubHub.name
