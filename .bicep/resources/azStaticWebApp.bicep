param name string
param location string
param sku string

var skus = {
  Free: {
    name: 'Free'
    tier: 'F1'
  }
  Standard: {
    capacity: 1
    name: 'Standard'
    tier: 'Standard'
  }
}

resource azStaticWebApp 'Microsoft.Web/staticSites@2022-09-01' = {
  name: name
  location: location
  sku: skus[sku]
  properties: {}
}

output id string = azStaticWebApp.id
output name string = azStaticWebApp.name
output defaultHostName string = azStaticWebApp.properties.defaultHostname
