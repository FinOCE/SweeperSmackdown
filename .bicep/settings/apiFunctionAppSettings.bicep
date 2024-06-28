param functionAppName string
@secure()
param applicationInsightsInstrumentationKey string
param storageValueIsConnectionString bool
@secure()
param storageValue string
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
    AzureWebJobsStorage: storageValueIsConnectionString ? storageValue : null
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: storageValueIsConnectionString ? storageValue : null
    WEBSITE_CONTENTSHARE: functionAppName
    AzureWebJobsStorage__accountName: storageValueIsConnectionString ? null : storageValue
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    FUNCTIONS_EXTENSION_VERSION: '~4'
    APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsightsInstrumentationKey
    CosmosDbConnectionString: cosmosDbConnectionString
    WebPubsubConnectionString: webPubSubConnectionString
    BearerTokenSecretKey: bearerTokenSecretKey
    DiscordClientId: discordClientId
    DiscordClientSecret: discordClientSecret
  }
}
