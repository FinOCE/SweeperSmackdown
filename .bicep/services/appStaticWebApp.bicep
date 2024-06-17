targetScope = 'subscription'

param location string
param environment string
param resourceGroupName string

var staticWebAppName = 'swa-sweepersmackdown-${environment}'

resource azResourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' existing = {
  name: resourceGroupName
}

module azStaticWebApp '../resources/azStaticWebApp.bicep' = {
  scope: azResourceGroup
  name: 'staticWebApp'
  params: {
    name: staticWebAppName
    location: location
  }
}
