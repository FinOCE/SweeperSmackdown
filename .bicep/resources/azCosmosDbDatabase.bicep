param name string
param location string
param resourceName string

resource azCosmosDb 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: resourceName
}

resource azCosmosDbDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-11-15' = {
  parent: azCosmosDb
  name: name
  location: location
  properties: {
    resource: {
      id: name
    }
  }
}

output id string = azCosmosDbDatabase.id
output name string = azCosmosDbDatabase.name
