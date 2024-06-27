param location string
param environment string
@secure()
param bearerTokenSecretKey string
param discordClientId string
@secure()
param discordClientSecret string

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
var applicationInsightsName = 'ai-api-${environment}-${resourceToken}'
var serverFarmName = 'sf-api-${environment}-${resourceToken}'
var storageAccountName = 'saapi${environment}${resourceToken}'
var storageContainerName = 'app-package-${take(resourceToken, 7)}-${resourceToken}'
var functionAppName = 'fa-api-${environment}-${resourceToken}'

module azCosmosDb '../resources/azCosmosDb.bicep' = {
  name: cosmosDbName
  params: {
    name: cosmosDbName
    location: location
  }
}

module azCosmosDbDatabase '../resources/azCosmosDbDatabase.bicep' = {
  name: databaseName
  params: {
    name: databaseName
    location: location
    resourceName: azCosmosDb.outputs.name
  }
}

module azCosmosDbContainer '../resources/azCosmosDbContainer.bicep' = [for container in containers: {
  name: container.name
  params: {
    name: container.name
    location: location
    partitionKeyPath: container.partitionKeyPath
    databaseName: azCosmosDbDatabase.outputs.name
  }
}]

module azWebPubSub '../resources/azWebPubSub.bicep' = {
  name: webPubSubName
  params: {
    name: webPubSubName
    location: location
    sku: 'Free'
  }
}

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
    location: location
    sku: 'Consumption'
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
    location: location
    serverFarmId: azServerFarm.outputs.id
    storageAccountName: azStorageAccount.outputs.name
    storageContainerName: azStorageContainer.outputs.name
    runtime: 'dotnet'
    isFlex: azServerFarm.outputs.isFlex
  }
}

module azFunctionAppSystemKey '../resources/azFunctionAppSystemKey.bicep' = {
  name: 'webpubsub_extension'
  params: {
    name: 'webpubsub_extension'
    functionAppName: azFunctionApp.outputs.name
  }
}

resource azStorageAccountExisting 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource azCosmosDbExisting 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: cosmosDbName
}

resource azWebPubSubExisting 'Microsoft.SignalRService/webPubSub@2023-02-01' existing = {
  name: webPubSubName
}

module apiFunctionAppSettings '../settings/apiFunctionAppSettings.bicep' = {
  name: '${functionAppName}-appsettings'
  params: {
    functionAppName: azFunctionApp.outputs.name
    applicationInsightsInstrumentationKey: azApplicationInsights.outputs.instrumentationKey
    storageConnectionString: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${azStorageAccountExisting.listKeys().keys[0].value}'
    cosmosDbConnectionString: azCosmosDbExisting.listConnectionStrings().connectionStrings[0].connectionString
    webPubSubConnectionString: azWebPubSubExisting.listKeys().primaryConnectionString
    bearerTokenSecretKey: bearerTokenSecretKey
    discordClientId: discordClientId
    discordClientSecret: discordClientSecret
  }
}

output name string = azFunctionApp.outputs.name
output defaultHostName string = azFunctionApp.outputs.defaultHostName
output webPubSubDefaultHostName string = azWebPubSubExisting.properties.hostName
