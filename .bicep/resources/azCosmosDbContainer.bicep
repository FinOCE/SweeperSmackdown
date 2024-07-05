param name string
param location string
param partitionKeyPath string
param databaseName string

resource azCosmosDbContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-11-15' = {
  name: '${databaseName}/${name}'
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
