targetScope = 'subscription'

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

var resourceGroup = 'rg-sweepersmackdown-${environment}'

resource azResourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroup
  location: location
}

module apiFunctionApp 'services/apiFunctionApp.bicep' = {
  name: 'apiFunctionApp'
  scope: azResourceGroup
  params: {
    location: location
    environment: environment
    bearerTokenSecretKey: bearerTokenSecretKey
    discordClientId: discordClientId
    discordClientSecret: discordClientSecret
  }
}

module botFunctionApp 'services/botFunctionApp.bicep' = {
  name: 'botFunctionApp'
  scope: azResourceGroup
  params: {
    location: location
    environment: environment
    discordPublicKey: discordPublicKey
  }
}

module appStaticWebApp 'services/appStaticWebApp.bicep' = {
  name: 'appStaticWebApp'
  scope: azResourceGroup
  params: {
    location: location
    environment: environment
  }
}

output resourceGroupName string = azResourceGroup.name
output apiFunctionAppName string = apiFunctionApp.outputs.name
output apiFunctionAppDefaultHostName string = apiFunctionApp.outputs.defaultHostName
output botFunctionAppName string = botFunctionApp.outputs.name
output botFunctionAppDefaultHostName string = botFunctionApp.outputs.defaultHostName
output staticWebAppName string = appStaticWebApp.outputs.name
output staticWebAppDefaultHostName string = appStaticWebApp.outputs.defaultHostName
