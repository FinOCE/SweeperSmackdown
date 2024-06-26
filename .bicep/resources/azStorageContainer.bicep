param name string
@allowed(['Blob', 'Container', 'None'])
param publicAccess string
param storageAccountName string

resource azStorageBlobServices 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
    name: '${storageAccountName}/default'
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
