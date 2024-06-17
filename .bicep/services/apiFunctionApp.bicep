targetScope = 'subscription'

param location string
param environment string
param resourceGroupName string
@allowed(['Consumption', 'FlexConsumption'])
param serverFarmSku string
param runtime string
param cosmosDbName string
@secure()
param bearerTokenSecretKey string
@secure()
param discordClientId string
@secure()
param discordClientSecret string
@allowed(['Free', 'Standard'])
param webPubSubSku string
@allowed(['dotnet', 'dotnet-isolated'])
param eventHandlerAddress string

var resourceToken = toLower(uniqueString(subscription().id, environment, location, functionAppName)) // TODO: Use resource token for all names

var webPubSubName = 'ws-sweepersmackdown-${environment}'
var webPubSubHubName = 'Game'
var applicationInsightsName = 'ai-sweepersmackdown-${environment}'
var serverFarmName = 'sf-sweepersmackdown-${environment}'
var storageAccountName = 'sasweepersmackdown${environment}'
var storageContainerName = 'app-package-${take(resourceToken, 7)}'
var functionAppName = 'fa-sweepersmackdown-${environment}'

resource azResourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' existing = {
  name: resourceGroupName
}

module azWebPubSub '../resources/azWebPubSub.bicep' = {
  name: webPubSubName
  scope: azResourceGroup
  params: {
    name: webPubSubName
    location: location
    sku: webPubSubSku
  }
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

module azWebPubSubHub '../resources/azWebPubSubHub.bicep' = {
  name: webPubSubHubName
  scope: azResourceGroup
  params: {
    name: webPubSubHubName
    webPubSubName: azWebPubSub.outputs.name
    functionAppName: azFunctionApp.outputs.name
    eventHandlerAddress: eventHandlerAddress
  }
}

resource azCosmosDbExisting 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: cosmosDbName
  scope: azResourceGroup
}

resource azWebPubSubExisting 'Microsoft.SignalRService/webPubSub@2023-02-01' existing = {
  name: webPubSubName
  scope: azResourceGroup
}

module apiFunctionAppSettings '../settings/apiFunctionAppSettings.bicep' = {
  name: '${functionAppName}/appsettings'
  scope: azResourceGroup
  params: {
    functionAppName: azFunctionApp.outputs.name
    storageAccountName: azStorageAccount.outputs.name
    applicationInsightsInstrumentationKey: azApplicationInsights.outputs.instrumentationKey
    cosmosDbConnectionString: azCosmosDbExisting.listConnectionStrings().connectionStrings[0].connectionString
    webPubSubConnectionString: azWebPubSubExisting.listKeys().primaryConnectionString
    bearerTokenSecretKey: bearerTokenSecretKey
    discordClientId: discordClientId
    discordClientSecret: discordClientSecret
  }
}
