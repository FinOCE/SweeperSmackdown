param name string
@allowed(['Blob', 'Container', 'None'])
param publicAccess string
param storageAccountName string

resource azStorageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: storageAccountName
}

resource azStorageBlobServices 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
    name: 'default'
    parent: azStorageAccount
}

resource azStorageContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  name: name
  parent: azStorageBlobServices
  properties: {
    publicAccess: publicAccess
  }
}

output id string = azStorageContainer.id
output name string = azStorageContainer.name
