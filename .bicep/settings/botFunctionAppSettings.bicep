param functionAppName string
param runtime string
@secure()
param storageValue string
@secure()
param applicationInsightsInstrumentationKey string
param discordPublicKey string

resource azFunctionApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: functionAppName
}

var skuSettings = azFunctionApp.kind == 'functionapp'
  ? {
    AzureWebJobsStorage: storageValue
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: storageValue
    WEBSITE_CONTENTSHARE: functionAppName
    FUNCTIONS_WORKER_RUNTIME: runtime
    FUNCTIONS_EXTENSION_VERSION: '~4'
  }
  : {
    AzureWebJobsStorage__accountName: storageValue
  }

resource azFunctionAppSettings 'Microsoft.Web/sites/config@2023-12-01' = {
  name: 'appsettings'
  parent: azFunctionApp
  properties: union(skuSettings, {
    APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsightsInstrumentationKey
    DISCORD_PUBLIC_KEY: discordPublicKey
  })
}
