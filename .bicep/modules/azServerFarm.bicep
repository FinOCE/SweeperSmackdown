// Parameters
param name string
param location string

// Create server farm
resource azServerFarm 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: name
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

// Outputs
output id string = azServerFarm.id
