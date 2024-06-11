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

@description('The secret key used to hash bearer tokens')
@secure()
param bearerTokenSecretKey string

@description('The client ID of the Discord application')
param discordClientId string

@description('The client secret of the Discord application')
@secure()
param discordClientSecret string

@description('The public key of the Discord application')
param discordPublicKey string

// Names
var product = 'sweepersmackdown'

var resourceGroup = 'rg-${product}-${environment}'
var cosmosDb = 'db-${product}-${environment}'
var webPubsub = 'ws-${product}-${environment}'
var applicationInsightsApi = 'ai-${product}-${environment}'
var applicationInsightsBot = 'ai-bot-${product}-${environment}'
var serverFarmApi = 'sf-${product}-${environment}'
var serverFarmBot = 'sf-bot-${environment}'
var storageAccountApi = 'sa${product}${environment}'
var storageAccountBot = 'sabot${environment}'
var functionAppApi = 'fa-${product}-${environment}'
var functionAppBot = 'fa-bot-${environment}'
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
    nameApi: applicationInsightsApi
    nameBot: applicationInsightsBot
    location: location
  }
}

// Create server farm
module azServerFarm 'modules/azServerFarm.bicep' = {
  scope: azResourceGroup
  name: 'serverFarm'
  params: {
    nameApi: serverFarmApi
    nameBot: serverFarmBot
    location: location
  }
}

// Create storage account
module azStorageAccount 'modules/azStorageAccount.bicep' = {
  scope: azResourceGroup
  name: 'storageAccount'
  params: {
    nameApi: storageAccountApi
    nameBot: storageAccountBot
    location: location
  }
}

// Create function app
module azFunctionApp 'modules/azFunctionApp.bicep' = {
  scope: azResourceGroup
  name: 'functionApp'
  params: {
    location: location

    // Api params
    nameApi: functionAppApi
    cosmosDbName: azCosmosDb.outputs.name
    webPubsubName: azWebPubsub.outputs.name
    apiApplicationInsightsInstrumentationKey: azApplicationInsights.outputs.apiInstrumentationKey
    apiServerFarmId: azServerFarm.outputs.apiId
    apiStorageName: azStorageAccount.outputs.apiName

    bearerTokenSecretKey: bearerTokenSecretKey
    discordClientId: discordClientId
    discordClientSecret: discordClientSecret

    // Bot params
    nameBot: functionAppBot
    botApplicationInsightsInstrumentationKey: azApplicationInsights.outputs.botInstrumentationKey
    botServerFarmId: azServerFarm.outputs.botId
    botStorageName: azStorageAccount.outputs.botName

    discordPublicKey: discordPublicKey
  }
}

var azFunctionAppDefaultHostName = azFunctionApp.outputs.apiDefaultHostName

// Create web pubsub hub
module azWebPubsubHub 'modules/azWebPubsubHub.bicep' = {
  scope: azResourceGroup
  name: 'webPubsubHub'
  params: {
    webPubsubName: webPubsub
    eventHandlerAddress: 'https://${azFunctionAppDefaultHostName}/runtime/webhooks/webpubsub'
    functionAppName: azFunctionApp.outputs.apiName
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
output apiFunctionAppName string = azFunctionApp.outputs.apiName
output apiFunctionAppDefaultHostName string = azFunctionApp.outputs.apiDefaultHostName
output botFunctionAppName string = azFunctionApp.outputs.botName
output botFunctionAppDefaultHostName string = azFunctionApp.outputs.botDefaultHostName
output staticWebAppName string = azStaticWebApp.outputs.name
output staticWebAppDefaultHostName string = azStaticWebApp.outputs.defaultHostName
