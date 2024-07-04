param functionAppName string
param runtime string
@secure()
param storageValue string
@secure()
param applicationInsightsInstrumentationKey string
@secure()
param secrets object

resource azFunctionApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: functionAppName
}

var inProcessSettings = {
  AzureWebJobsStorage: storageValue
  WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: storageValue
  WEBSITE_CONTENTSHARE: functionAppName
  WEBSITE_RUN_FROM_PACKAGE: 1
}

var isolatedProcessSettings = {
    AzureWebJobsStorage__accountName: storageValue
}

resource azFunctionAppSettings 'Microsoft.Web/sites/config@2023-12-01' = {
  name: 'appsettings'
  parent: azFunctionApp
  properties: union(
    azFunctionApp.kind == 'functionapp' ? inProcessSettings : isolatedProcessSettings,
    {
      APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsightsInstrumentationKey
      FUNCTIONS_WORKER_RUNTIME: runtime
      FUNCTIONS_EXTENSION_VERSION: '~4'
    },
    secrets
  )
}
