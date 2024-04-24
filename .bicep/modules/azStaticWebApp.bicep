// Parameters
param name string
param location string

// Create static web app
resource azStaticWebApp 'Microsoft.Web/staticSites@2022-09-01' = {
  name: name
  location: location
  sku: {
    name: 'Free'
    tier: 'F1'
  }
  properties: {}
}

// Outputs
output name string = azStaticWebApp.name
output defaultHostName string = azStaticWebApp.properties.defaultHostname
