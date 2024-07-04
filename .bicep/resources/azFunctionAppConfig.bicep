param functionAppName string
param runtime string
param storageAccountName string
@secure()
param applicationInsightsInstrumentationKey string
@secure()
param secrets object

resource azFunctionApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: functionAppName
}

resource azStorageAccountExisting 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

var kindSettings = {
  'functionapp,linux': {
    AzureWebJobsStorage__accountName: azStorageAccountExisting.name
  }
  functionapp: {
    AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${azStorageAccountExisting.listKeys().keys[0].value}'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${azStorageAccountExisting.listKeys().keys[0].value}'
    WEBSITE_CONTENTSHARE: functionAppName
    WEBSITE_RUN_FROM_PACKAGE: 1
    FUNCTIONS_WORKER_RUNTIME: runtime
    FUNCTIONS_EXTENSION_VERSION: '~4'
  }
}

var runtimeSettings = {
  'dotnet-isolated': {
    WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED: 1
  }
  dotnet: {
    SCM_DO_BUILD_DURING_DEPLOYMENT: true
  }
}

resource azFunctionAppSettings 'Microsoft.Web/sites/config@2023-12-01' = {
  name: 'appsettings'
  parent: azFunctionApp
  properties: union(
    kindSettings[azFunctionApp.kind],
    runtimeSettings[runtime],
    {
      APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsightsInstrumentationKey
    },
    secrets
  )
}
