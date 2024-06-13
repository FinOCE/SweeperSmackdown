namespace SweeperSmackdown.Bot.Services

type ISigningService =
    abstract member Verify:
        message: string
        * signature: string
        * publicKey: string
        -> bool
