param name string
param location string
param serverFarmId string
param storageAccountName string
param storageContainerName string
@allowed(['dotnet', 'dotnet-isolated'])
param runtime string

resource azStorageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource azFunctionApp 'Microsoft.Web/sites@2023-12-01' = {
  name: name
  location: location
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: serverFarmId
    httpsOnly: true
    siteConfig: {
      cors: {
        allowedOrigins: ['*']
      }
    }
    functionAppConfig: {
      scaleAndConcurrency: {
        maximumInstanceCount: 100
        instanceMemoryMB: 2048
      }
      runtime: { 
        name: runtime
        version: runtime == 'dotnet-isolated' ? '8.0' : '6.0'
      }
      deployment: {
        storage: {
          type: 'blobContainer'
          value: '${azStorageAccount.properties.primaryEndpoints.blob}${storageContainerName}'
          authentication: {
            type: 'SystemAssignedIdentity'
          }
        }
      }
    }
  }
}

var storageRoleDefinitionId = 'b7e6dc6d-f1e8-4753-8033-0f276bb0955b' // Built-in blob storage role data owner role

resource azRoleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(azStorageAccount.id, storageRoleDefinitionId)
  scope: azStorageAccount
  properties: {
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', storageRoleDefinitionId)
    principalId: azFunctionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

output id string = azFunctionApp.id
output name string = azFunctionApp.name
output defaultHostName string = azFunctionApp.properties.defaultHostName
