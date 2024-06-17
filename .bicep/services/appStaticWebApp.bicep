param location string
param environment string

var resourceToken = take(toLower(uniqueString(subscription().id, environment, location, 'app')), 7)

var staticWebAppName = 'swa-app-${environment}-${resourceToken}'

module azStaticWebApp '../resources/azStaticWebApp.bicep' = {
  name: staticWebAppName
  params: {
    name: staticWebAppName
    location: location
  }
}

output name string = azStaticWebApp.outputs.name
output defaultHostName string = azStaticWebApp.outputs.defaultHostName
