param functionAppName string
param storageAccountName string
@secure()
param applicationInsightsInstrumentationKey string
param discordPublicKey string

resource azFunctionAppSettings 'Microsoft.Web/sites/config@2023-12-01' = {
  name: '${functionAppName}/appsettings'
  properties: {
    AzureWebJobsStorage__accountName: storageAccountName
    APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsightsInstrumentationKey
    DISCORD_PUBLIC_KEY: discordPublicKey
  }
}
