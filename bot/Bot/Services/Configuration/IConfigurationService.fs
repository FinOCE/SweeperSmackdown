﻿namespace SweeperSmackdown.Bot.Services

type IConfigurationService =
    abstract member TryGetValue: key: string -> string option
