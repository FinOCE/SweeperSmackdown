param name string
param location string
param resourceName string

resource azCosmosDbDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-11-15' = {
  name: '${resourceName}/${name}'
  location: location
  properties: {
    resource: {
      id: name
    }
  }
}

output id string = azCosmosDbDatabase.id
output name string = azCosmosDbDatabase.name
