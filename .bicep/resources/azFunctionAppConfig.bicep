param functionAppName string
param runtime string
@secure()
param storageValue string
@secure()
param applicationInsightsInstrumentationKey string
@secure()
param secrets object

var runtimeSettings = {
  'dotnet-isolated': {
    AzureWebJobsStorage__accountName: storageValue
    WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED: 1
  }
  dotnet: {
    AzureWebJobsStorage: storageValue
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: storageValue
    WEBSITE_CONTENTSHARE: functionAppName
    WEBSITE_RUN_FROM_PACKAGE: 1
  }
}

resource azFunctionAppSettings 'Microsoft.Web/sites/config@2023-12-01' = {
  name: '${functionAppName}/appsettings'
  properties: union(
    runtimeSettings[runtime],
    {
      APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsightsInstrumentationKey
      FUNCTIONS_WORKER_RUNTIME: runtime
      FUNCTIONS_EXTENSION_VERSION: '~4'
    },
    secrets
  )
}
