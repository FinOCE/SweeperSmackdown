param location string
param environment string
param sku string

param discordPublicKey string

var resourceToken = take(toLower(uniqueString(subscription().id, environment, location, 'bot')), 7)

var applicationInsightsName = 'ai-bot-${environment}-${resourceToken}'
var serverFarmName = 'sf-bot-${environment}-${resourceToken}'
var storageAccountName = 'sabot${environment}${resourceToken}'
var storageContainerName = 'app-package-${resourceToken}'
var functionAppName = 'fa-bot-${environment}-${resourceToken}'

module azApplicationInsights '../resources/azApplicationInsights.bicep' = {
  name: applicationInsightsName
  params: {
    name: applicationInsightsName
    location: location
  }
}

module azServerFarm '../resources/azServerFarm.bicep' = {
  name: serverFarmName
  params: {
    name: serverFarmName
    sku: sku
  }
}

module azStorageAccount '../resources/azStorageAccount.bicep' = {
  name: storageAccountName
  params: {
    name: storageAccountName
    location: location
  }
}

module azStorageContainer '../resources/azStorageContainer.bicep' = {
  name: storageContainerName
  params: {
    name: storageContainerName
    publicAccess: 'None'
    storageAccountName: azStorageAccount.outputs.name
  }
}

module azFunctionApp '../resources/azFunctionApp.bicep' = {
  name: functionAppName
  params: {
    name: functionAppName
    runtime: 'dotnet-isolated'
    version: '8.0'
    sku: sku
    serverFarmId: azServerFarm.outputs.id
    storageAccountName: azStorageAccount.outputs.name
    storageContainerName: azStorageContainer.outputs.name
  }
}

resource azStorageAccountExisting 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

module botFunctionAppSettings '../settings/botFunctionAppSettings.bicep' = {
  name: '${functionAppName}-appsettings'
  params: {
    functionAppName: azFunctionApp.outputs.name
    runtime: 'dotnet-isolated'
    storageValue: sku != 'FlexConsumption' ? 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${azStorageAccountExisting.listKeys().keys[0].value}' : azStorageAccount.outputs.name
    applicationInsightsInstrumentationKey: azApplicationInsights.outputs.instrumentationKey
    discordPublicKey: discordPublicKey
  }
}

output name string = azFunctionApp.outputs.name
output defaultHostName string = azFunctionApp.outputs.defaultHostName
