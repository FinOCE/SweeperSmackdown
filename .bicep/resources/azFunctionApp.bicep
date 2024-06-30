param name string
param serverFarmId string
param storageAccountName string
param storageContainerName string
param runtime string
param version string
param sku string

var isWindows = sku == 'Consumption'

resource azStorageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource azFunctionAppWindows 'Microsoft.Web/sites@2023-12-01' = if (isWindows) {
  name: name
  location: resourceGroup().location
  kind: 'functionapp'
  properties: {
    serverFarmId: serverFarmId
    httpsOnly: true
    siteConfig: {
      cors: {
        allowedOrigins: ['*']
      }
    }
  }
}

resource azFunctionAppLinux 'Microsoft.Web/sites@2023-12-01' = if (!isWindows) {
  name: name
  location: resourceGroup().location
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
  }
}

var storageRoleDefinitionId = 'b7e6dc6d-f1e8-4753-8033-0f276bb0955b' // Built-in blob storage role data owner role

resource azRoleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = if (!isWindows) {
  name: guid(azStorageAccount.id, storageRoleDefinitionId)
  scope: azStorageAccount
  properties: {
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', storageRoleDefinitionId)
    principalId: azFunctionAppLinux.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

output id string = isWindows ? azFunctionAppWindows.id : azFunctionAppLinux.id
output name string = isWindows ? azFunctionAppWindows.name : azFunctionAppLinux.name
output defaultHostName string = isWindows ? azFunctionAppWindows.properties.defaultHostName : azFunctionAppLinux.properties.defaultHostName
