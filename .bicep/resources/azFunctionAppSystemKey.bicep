param name string
param functionAppName string

resource azFunctionAppSystemKey 'Microsoft.Web/sites/host/systemkeys@2021-03-01' = {
  name: '${functionAppName}/default/${name}'
  properties: {
    name: name
  }
}
