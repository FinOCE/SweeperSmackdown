targetScope = 'subscription'

param location string
param environment string
param resourceGroupName string

var name = 'db-sweepersmackdown-${environment}'
var databaseName = 'smackdown-db'
var containers = [
  {
    name: 'lobbies'
    partitionkeyPath: '/id'
  }
  {
    name: 'players'
    partitionKeyPath: '/lobbyId'
  }
]

resource azResourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' existing = {
  name: resourceGroupName
}

module azCosmosDb '../resources/azCosmosDb.bicep' = {
  name: name
  scope: azResourceGroup
  params: {
    name: name
    location: location
  }
}

module azCosmosDbDatabase '../resources/azCosmosDbDatabase.bicep' = {
  name: databaseName
  scope: azResourceGroup
  params: {
    name: databaseName
    location: location
    resourceName: azCosmosDb.outputs.name
  }
}

module azCosmosDbContainer '../resources/azCosmosDbContainer.bicep' = [for container in containers: {
  name: container.name
  scope: azResourceGroup
  params: {
    name: container.name
    location: location
    partitionKeyPath: container.partitionKeyPath
    databaseName: azCosmosDbDatabase.outputs.name
  }
}]

output id string = azCosmosDb.outputs.id
output name string = azCosmosDb.outputs.name
