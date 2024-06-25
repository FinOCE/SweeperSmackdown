param name string
param webPubSubName string
param functionAppName string

resource azWebPubSub 'Microsoft.SignalRService/webPubSub@2023-02-01' existing = {
  name: webPubSubName
}

resource azFunctionApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: functionAppName
}

var eventHandlerAddress = azFunctionApp.properties.defaultHostName
var extensionKey = listkeys('${resourceId('Microsoft.Web/sites', functionAppName)}/host/default/','2022-09-01').systemkeys.webpubsub_extension

resource azWebPubSubHub 'Microsoft.SignalRService/webPubSub/hubs@2023-02-01' = {
  name: name
  parent: azWebPubSub
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
