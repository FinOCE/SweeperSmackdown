param name string
param location string

resource azStaticWebApp 'Microsoft.Web/staticSites@2022-09-01' = {
  name: name
  location: location
  sku: {
    name: 'Free'
    tier: 'F1'
  }
  properties: {}
}

output id string = azStaticWebApp.id
output name string = azStaticWebApp.name
output defaultHostName string = azStaticWebApp.properties.defaultHostname
