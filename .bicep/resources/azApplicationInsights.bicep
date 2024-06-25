param name string
param location string

resource azApplicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: name
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

output id string = azApplicationInsights.id
output name string = azApplicationInsights.name
output instrumentationKey string = azApplicationInsights.properties.InstrumentationKey
