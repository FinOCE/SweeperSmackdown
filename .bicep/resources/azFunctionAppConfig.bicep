param functionAppName string
param runtime string
param storageAccountName string
@secure()
param applicationInsightsInstrumentationKey string
@secure()
param secrets object

resource azStorageAccountExisting 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

var runtimeSettings = {
  'dotnet-isolated': {
    AzureWebJobsStorage__accountName: azStorageAccountExisting.name
    WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED: 1
  }
  dotnet: {
    AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${azStorageAccountExisting.listKeys().keys[0].value}'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${azStorageAccountExisting.listKeys().keys[0].value}'
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
