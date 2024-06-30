param name string
@allowed(['Consumption', 'FlexConsumption'])
param sku string

var isWindows = sku == 'Consumption'

resource azServerFarmWindows 'Microsoft.Web/serverfarms@2023-12-01' = if (isWindows) {
  name: name
  location: resourceGroup().location
  kind: 'functionapp'
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

resource azServerFarmLinux 'Microsoft.Web/serverfarms@2023-12-01' = if (!isWindows) {
  name: name
  location: resourceGroup().location
  kind: 'functionapp,linux'
  sku: {
    name: 'FC1'
    tier: 'FlexConsumption'
  }
  properties: {
    reserved: true
  }
}

output id string = isWindows ? azServerFarmWindows.id : azServerFarmLinux.id
output name string = isWindows ? azServerFarmWindows.name : azServerFarmLinux.name
