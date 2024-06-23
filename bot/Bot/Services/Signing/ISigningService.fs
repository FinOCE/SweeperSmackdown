namespace SweeperSmackdown.Bot.Services

type ISigningService =
    abstract member Verify:
        timestamp: string
        * body: string
        * signature: string
        * publicKey: string
        -> bool
