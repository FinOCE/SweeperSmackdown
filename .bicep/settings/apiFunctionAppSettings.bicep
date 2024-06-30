param functionAppName string
@secure()
param applicationInsightsInstrumentationKey string
@secure()
param storageConnectionString string
@secure()
param cosmosDbConnectionString string
@secure()
param webPubSubConnectionString string
@secure()
param bearerTokenSecretKey string
param discordClientId string
@secure()
param discordClientSecret string

resource azFunctionAppSettings 'Microsoft.Web/sites/config@2023-12-01' = {
  name: '${functionAppName}/appsettings'
  properties: {
    AzureWebJobsStorage: storageConnectionString
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsightsInstrumentationKey
    CosmosDbConnectionString: cosmosDbConnectionString
    WebPubsubConnectionString: webPubSubConnectionString
    BearerTokenSecretKey: bearerTokenSecretKey
    DiscordClientId: discordClientId
    DiscordClientSecret: discordClientSecret
  }
}
