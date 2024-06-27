param name string
param webPubSubName string
param functionAppName string

resource azFunctionApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: functionAppName
}

var eventHandlerAddress = azFunctionApp.properties.defaultHostName
var extensionKey = listKeys(resourceId('Microsoft.Web/sites/host', functionAppName, 'default'), '2022-03-01').systemkeys.webpubsub_extension

resource azWebPubSubHub 'Microsoft.SignalRService/webPubSub/hubs@2023-02-01' = {
  name: '${webPubSubName}/${name}'
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

output id string = azWebPubSubHub.id
output name string = azWebPubSubHub.name
