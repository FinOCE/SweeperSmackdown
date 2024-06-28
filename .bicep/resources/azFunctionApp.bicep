param name string
param location string
param serverFarmId string
param storageAccountName string
param storageContainerName string
param runtime string
param version string
param sku string

resource azStorageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource azFunctionApp 'Microsoft.Web/sites@2023-12-01' = {
  name: name
  location: location
  kind: sku == 'FlexConsumption' ? 'functionapp,linux' : 'functionapp'
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
    functionAppConfig: sku == 'FlexConsumption'
      ? {
        scaleAndConcurrency: {
          maximumInstanceCount: 100
          instanceMemoryMB: 2048
        }
        runtime: { 
          name: runtime
          version: version
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
      : null
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
