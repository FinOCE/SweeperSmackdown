param name string
param location string
@allowed(['Consumption', 'FlexConsumption'])
param sku string

var skus = {
  Consumption: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  FlexConsumption: {
    name: 'FC1'
    tier: 'FlexConsumption'
  }
}

resource azServerFarm 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: name
  location: location
  kind: 'functionapp,linux'
  sku: skus[sku]
  properties: {
    reserved: true
  }
}

output id string = azServerFarm.id
output name string = azServerFarm.name
