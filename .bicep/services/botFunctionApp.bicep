targetScope = 'subscription'

param location string
param environment string
param resourceGroupName string
@allowed(['Consumption', 'FlexConsumption'])
param serverFarmSku string
param runtime string
param discordPublicKey string

var resourceToken = toLower(uniqueString(subscription().id, environment, location, functionAppName)) // TODO: Use resource token for all names

var applicationInsightsName = 'ai-sweeperbot-${environment}'
var serverFarmName = 'sf-sweeperbot-${environment}'
var storageAccountName = 'sasweeperbot${environment}'
var storageContainerName = 'app-package-${take(resourceToken, 7)}'
var functionAppName = 'fa-sweeperbot-${environment}'

resource azResourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' existing = {
  name: resourceGroupName
}

module azApplicationInsights '../resources/azApplicationInsights.bicep' = {
  name: applicationInsightsName
  scope: azResourceGroup
  params: {
    name: applicationInsightsName
    location: location
  }
}

module azServerFarm '../resources/azServerFarm.bicep' = {
  name: serverFarmName
  scope: azResourceGroup
  params: {
    name: serverFarmName
    location: location
    sku: serverFarmSku
  }
}

module azStorageAccount '../resources/azStorageAccount.bicep' = {
  name: storageAccountName
  scope: azResourceGroup
  params: {
    name: storageAccountName
    location: location
  }
}

module azStorageContainer '../resources/azStorageContainer.bicep' = {
  name: storageContainerName
  scope: azResourceGroup
  params: {
    name: storageContainerName
    publicAccess: 'None'
    storageAccountName: azStorageAccount.outputs.name
  }
}

module azFunctionApp '../resources/azFunctionApp.bicep' = {
  name: functionAppName
  scope: azResourceGroup
  params: {
    name: functionAppName
    location: location
    serverFarmId: azServerFarm.outputs.id
    storageAccountName: azStorageAccount.outputs.name
    storageContainerName: azStorageContainer.outputs.name
    runtime: runtime
  }
}

module botFunctionAppSettings '../settings/botFunctionAppSettings.bicep' = {
  name: '${functionAppName}/appsettings'
  scope: azResourceGroup
  params: {
    functionAppName: azFunctionApp.outputs.name
    storageAccountName: azStorageAccount.outputs.name
    applicationInsightsInstrumentationKey: azApplicationInsights.outputs.instrumentationKey
    discordPublicKey: discordPublicKey
  }
}
