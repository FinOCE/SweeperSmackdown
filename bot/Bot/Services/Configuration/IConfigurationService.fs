namespace SweeperSmackdown.Bot.Services

type IConfigurationService =
    abstract member ReadOrThrow: key: string -> string
