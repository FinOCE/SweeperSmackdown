// Parameters
param nameApi string
param location string
param cosmosDbName string
param webPubsubName string
param apiStorageName string
param apiServerFarmId string

param nameBot string
param botStorageName string
param botServerFarmId string
param botStorageContainerName string

@secure()
param bearerTokenSecretKey string

param discordClientId string
param discordPublicKey string

@secure()
param discordClientSecret string

@secure()
param apiApplicationInsightsInstrumentationKey string

@secure()
param botApplicationInsightsInstrumentationKey string

var storageRoleDefinitionId = 'b7e6dc6d-f1e8-4753-8033-0f276bb0955b' // Built-in blob storage role data owner role

// Get storage account connection string api
resource azStorageAccountApi 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: apiStorageName
}

var azStorageAccountApiConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${apiStorageName};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${azStorageAccountApi.listKeys().keys[0].value}'

// Get storage account connection string bot
resource azStorageAccountBot 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: botStorageName
}

// Get database
resource azCosmosDb 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: cosmosDbName
}

var azCosmosDbConnectionString = azCosmosDb.listConnectionStrings().connectionStrings[0].connectionString

// Get web pubsub
resource azWebPubsub 'Microsoft.SignalRService/webPubSub@2023-02-01' existing = {
  name: webPubsubName
}

var webPubsubConnectionString = azWebPubsub.listKeys().primaryConnectionString

// Create function app api
resource azFunctionAppApi 'Microsoft.Web/sites@2022-09-01' = {
  name: nameApi
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: apiServerFarmId
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: azStorageAccountApiConnectionString
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: azStorageAccountApiConnectionString
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: nameApi
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: apiApplicationInsightsInstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
          value: 'true'
        }
        {
          name: 'CosmosDbConnectionString'
          value: azCosmosDbConnectionString
        }
        {
          name: 'WebPubsubConnectionString'
          value: webPubsubConnectionString
        }
        {
          name: 'BearerTokenSecretKey'
          value: bearerTokenSecretKey
        }
        {
          name: 'DiscordClientId'
          value: discordClientId
        }
        {
          name: 'DiscordClientSecret'
          value: discordClientSecret
        }
      ]
      cors: {
        allowedOrigins: [
          '*'
        ]
      }
    }
  }
}

// Create function app bot
resource azFunctionAppBot 'Microsoft.Web/sites@2023-12-01' = {
  name: nameBot
  location: location
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: botServerFarmId
    httpsOnly: true
    functionAppConfig: {
      scaleAndConcurrency: {
        maximumInstanceCount: 100
        instanceMemoryMB: 2048
      }
      runtime: { 
        name: 'dotnet-isolated'
        version: '8.0'
      }
      deployment: {
        storage: {
          type: 'blobContainer'
          value: '${azStorageAccountBot.properties.primaryEndpoints.blob}${botStorageContainerName}'
          authentication: {
            type: 'SystemAssignedIdentity'
          }
        }
      }
    }
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage__accountName'
          value: azStorageAccountBot.name
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: botApplicationInsightsInstrumentationKey
        }
        {
          name: 'DISCORD_PUBLIC_KEY'
          value: discordPublicKey
        }
      ]
      cors: {
        allowedOrigins: [
          '*'
        ]
      }
    }
  }
}

// Add access to storage account for bot
resource azRoleAssignmentBot 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(azStorageAccountBot.id, storageRoleDefinitionId)
  scope: azStorageAccountBot
  properties: {
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', storageRoleDefinitionId)
    principalId: azFunctionAppBot.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// Outputs
output apiName string = azFunctionAppApi.name
output apiDefaultHostName string = azFunctionAppApi.properties.defaultHostName
output botName string = azFunctionAppBot.name
output botDefaultHostName string = azFunctionAppBot.properties.defaultHostName
