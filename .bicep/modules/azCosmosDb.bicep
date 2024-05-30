// Parameters
param name string
param location string

// Create cosmos db
resource azCosmosDb 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: name
  kind: 'GlobalDocumentDB'
  location: location
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
  }
}

// Create database
resource azCosmosDbDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-11-15' = {
  parent: azCosmosDb
  name: 'smackdown-db'
  location: location
  properties: {
    resource: {
      id: 'smackdown-db'
    }
  }
}

// Create containers
resource azCosmosDbContainerLobbies 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-11-15' = {
  parent: azCosmosDbDatabase
  name: 'lobbies'
  location: location
  properties: {
    resource: {
      id: 'lobbies'
      partitionKey: {
        paths: ['/id']
        kind: 'Hash'
      }
    }
  }
}

resource azCosmosDbContainerVotes 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-11-15' = {
  parent: azCosmosDbDatabase
  name: 'votes'
  location: location
  properties: {
    resource: {
      id: 'votes'
      partitionKey: {
        paths: ['/id']
        kind: 'Hash'
      }
    }
  }
}

resource azCosmosDbContainerBoards 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-11-15' = {
  parent: azCosmosDbDatabase
  name: 'boards'
  location: location
  properties: {
    resource: {
      id: 'boards'
      partitionKey: {
        paths: ['/id']
        kind: 'Hash'
      }
    }
  }
}

resource azCosmosDbContainerAuth 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-11-15' = {
  parent: azCosmosDbDatabase
  name: 'auth'
  location: location
  properties: {
    resource: {
      id: 'auth'
      partitionKey: {
        paths: ['/id']
        kind: 'Hash'
      }
      defaultTtl: 86400 * 14
    }
  }
}

resource azCosmosDbContainerUsers 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-11-15' = {
  parent: azCosmosDbDatabase
  name: 'users'
  location: location
  properties: {
    resource: {
      id: 'users'
      partitionKey: {
        paths: ['/id']
        kind: 'Hash'
      }
    }
  }
}

// Outputs
output name string = azCosmosDb.name
