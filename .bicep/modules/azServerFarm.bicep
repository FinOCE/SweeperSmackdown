// Parameters
param nameApi string
param nameBot string
param location string

// Create server farm api
resource azServerFarmApi 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: nameApi
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

// Create server farm bot
resource azServerFarmBot 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: nameBot
  location: location
  kind: 'functionapp'
  sku: {
    name: 'Y1' // 'FC1'
    tier: 'Dynamic' // 'FlexConsumption'
  }
  // properties: {
  //   reserved: true
  // }
}

// Outputs
output apiId string = azServerFarmApi.id
output botId string = azServerFarmBot.id
