targetScope = 'subscription'

param location string
param environment string
param resourceGroupName string

var resourceToken = take(toLower(uniqueString(subscription().id, environment, location, 'app')), 7)

var staticWebAppName = 'swa-app-${environment}-${resourceToken}'

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
