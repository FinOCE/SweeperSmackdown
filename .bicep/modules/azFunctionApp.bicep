// Parameters
param name string
param location string
param cosmosDbName string
param webPubsubName string
param storageName string
param serverFarmId string

@secure()
param applicationInsightsInstrumentationKey string

// Get storage account connection string
resource azStorageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageName
}

var azStorageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageName};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${azStorageAccount.listKeys().keys[0].value}'

// Get database
resource azCosmosDb 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: cosmosDbName
}

var azCosmosDbConnectionString = azCosmosDb.listConnectionStrings().connectionStrings[0].connectionString

// Get web pubsub
resource azWebPubsub 'Microsoft.SignalRService/webPubSub@2023-02-01' existing = {
  name: webPubsubName
}

var webPubsubConnectionString = azWebPubsub.listConnectionStrings().connectionStrings[0].connectionString

// Create function app
resource azFunctionApp 'Microsoft.Web/sites@2022-09-01' = {
  name: name
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: serverFarmId
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: azStorageAccountConnectionString
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: azStorageAccountConnectionString
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: name
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsightsInstrumentationKey
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
      ]
      cors: {
        allowedOrigins: [
          '*'
        ]
      }
    }
  }
}

// Outputs
output name string = azFunctionApp.name
output defaultHostName string = azFunctionApp.properties.defaultHostName
