targetScope = 'subscription'

// Parameters
@description('The environment to build in')
@allowed([
  'dev'
  'prod'
])
param environment string

@description('The location to deploy to')
param location string

// Names
var product = 'sweepersmackdown'

var resourceGroup = 'rg-${product}-${environment}'
var cosmosDb = 'db-${product}-${environment}'
var webPubsub = 'ws-${product}-${environment}'
var applicationInsights = 'ai-${product}-${environment}'
var serverFarm = 'sf-${product}-${environment}'
var storageAccount = 'sa${product}${environment}'
var functionApp = 'fa-${product}-${environment}'
var staticWebApp = 'swa-${product}-${environment}'

// Create resource group
resource azResourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroup
  location: location
}

// Create cosmos db
module azCosmosDb 'modules/azCosmosDb.bicep' = {
  scope: azResourceGroup
  name: 'cosmosDb'
  params: {
    name: cosmosDb
    location: location
  }
}

// Create web pubsub
module azWebPubsub 'modules/azWebPubsub.bicep' = {
  scope: azResourceGroup
  name: 'webPubsub'
  params: {
    name: webPubsub
    location: location
  }
}

// Create application insights
module azApplicationInsights 'modules/azApplicationInsights.bicep' = {
  scope: azResourceGroup
  name: 'applicationInsights'
  params: {
    name: applicationInsights
    location: location
  }
}

// Create server farm
module azServerFarm 'modules/azServerFarm.bicep' = {
  scope: azResourceGroup
  name: 'serverFarm'
  params: {
    name: serverFarm
    location: location
  }
}

// Create storage account
module azStorageAccount 'modules/azStorageAccount.bicep' = {
  scope: azResourceGroup
  name: 'storageAccount'
  params: {
    name: storageAccount
    location: location
  }
}

// Create function app
module azFunctionApp 'modules/azFunctionApp.bicep' = {
  scope: azResourceGroup
  name: 'functionApp'
  params: {
    name: functionApp
    location: location
    cosmosDbName: azCosmosDb.outputs.name
    webPubsubName: azWebPubsub.outputs.name
    applicationInsightsInstrumentationKey: azApplicationInsights.outputs.instrumentationKey
    serverFarmId: azServerFarm.outputs.id
    storageName: azStorageAccount.outputs.name
  }
}

var azFunctionAppDefaultHostName = azFunctionApp.outputs.defaultHostName

// Create web pubsub hub
module azWebPubsubHub 'modules/azWebPubsubHub.bicep' = {
  scope: azResourceGroup
  name: 'webPubsubHub'
  params: {
    webPubsubName: webPubsub
    eventHandlerAddress: 'https://${azFunctionAppDefaultHostName}/runtime/webhooks/webpubsub'
  }
}

// Create static web app
module azStaticWebApp 'modules/azStaticWebApp.bicep' = {
  scope: azResourceGroup
  name: 'staticWebApp'
  params: {
    name: staticWebApp
    location: location
  }
}

// Outputs
output resourceGroupName string = azResourceGroup.name
output functionAppName string = azFunctionApp.outputs.name
output staticWebAppName string = azStaticWebApp.outputs.name
