param functionAppName string
param storageValueIsConnectionString bool
@secure()
param storageValue string
@secure()
param applicationInsightsInstrumentationKey string
param discordPublicKey string

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
    DISCORD_PUBLIC_KEY: discordPublicKey
  }
}
