// Parameters
param nameApi string
param nameBot string
param location string

// Create storage account api
resource azStorageAccountApi 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: nameApi
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
  }
}

// Create storage account bot
resource azStorageAccountBot 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: nameBot
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
  }
}

// Outputs
output apiName string = azStorageAccountApi.name
output botName string = azStorageAccountBot.name
