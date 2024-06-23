// Parameters
param nameApi string
param nameBot string
param location string

// Create application insights for API
resource azApplicationInsightsApi 'Microsoft.Insights/components@2020-02-02' = {
  name: nameApi
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

resource azApplicationInsightsBot 'Microsoft.Insights/components@2020-02-02' = {
  name: nameBot
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

// Outputs
output apiInstrumentationKey string = azApplicationInsightsApi.properties.InstrumentationKey
output botInstrumentationKey string = azApplicationInsightsBot.properties.InstrumentationKey
