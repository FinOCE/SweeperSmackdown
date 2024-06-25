param name string
param location string
param partitionKeyPath string
param databaseName string

resource azCosmosDbDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-11-15' existing = {
  name: databaseName
}

resource azCosmosDbContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-11-15' = {
  parent: azCosmosDbDatabase
  name: name
  location: location
  properties: {
    resource: {
      id: name
      partitionKey: {
        paths: [partitionKeyPath]
        kind: 'Hash'
      }
    }
  }
}

output id string = azCosmosDbContainer.id
output name string = azCosmosDbContainer.name
