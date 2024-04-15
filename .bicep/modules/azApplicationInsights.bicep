// Parameters
param name string
param location string

// Create application insights
resource azApplicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: name
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

// Outputs
output instrumentationKey string = azApplicationInsights.properties.InstrumentationKey
