targetScope = 'subscription'

param location string
param environment string
param resourceGroupName string
@secure()
param bearerTokenSecretKey string
@secure()
param discordClientId string
@secure()
param discordClientSecret string
@allowed(['dotnet', 'dotnet-isolated'])
param eventHandlerAddress string

var resourceToken = take(toLower(uniqueString(subscription().id, environment, location, 'api')), 7)

var cosmosDbName = 'db-api-${environment}-${resourceToken}'
var databaseName = 'smackdown-db'
var containers = [
  {
    name: 'lobbies'
    partitionkeyPath: '/id'
  }
  {
    name: 'players'
    partitionKeyPath: '/lobbyId'
  }
]

var webPubSubName = 'ws-api-${environment}-${resourceToken}'
var webPubSubHubName = 'Game'
var applicationInsightsName = 'ai-api-${environment}-${resourceToken}'
var serverFarmName = 'sf-api-${environment}-${resourceToken}'
var storageAccountName = 'saapi${environment}${resourceToken}'
var storageContainerName = 'app-package-${take(resourceToken, 7)}-${resourceToken}'
var functionAppName = 'fa-api-${environment}-${resourceToken}'

resource azResourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' existing = {
  name: resourceGroupName
}

module azCosmosDb '../resources/azCosmosDb.bicep' = {
  name: cosmosDbName
  scope: azResourceGroup
  params: {
    name: cosmosDbName
    location: location
  }
}

module azCosmosDbDatabase '../resources/azCosmosDbDatabase.bicep' = {
  name: databaseName
  scope: azResourceGroup
  params: {
    name: databaseName
    location: location
    resourceName: azCosmosDb.outputs.name
  }
}

module azCosmosDbContainer '../resources/azCosmosDbContainer.bicep' = [for container in containers: {
  name: container.name
  scope: azResourceGroup
  params: {
    name: container.name
    location: location
    partitionKeyPath: container.partitionKeyPath
    databaseName: azCosmosDbDatabase.outputs.name
  }
}]

module azWebPubSub '../resources/azWebPubSub.bicep' = {
  name: webPubSubName
  scope: azResourceGroup
  params: {
    name: webPubSubName
    location: location
    sku: 'Free'
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
    sku: 'Consumption'
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
    runtime: 'dotnet'
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
