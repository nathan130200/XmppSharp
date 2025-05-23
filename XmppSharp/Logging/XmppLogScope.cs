namespace XmppSharp.Logging;

#pragma warning disable format

[Flags]
public enum XmppLogScope : byte
{
    None       = 0b0000_0000,
    Connection = 0b0000_0001,
    Parser     = 0b0000_0010,
    Socket     = 0b0000_0100,
    Xmpp       = 0b0000_1000,
    State      = 0b0001_0000,
    Encryption = 0b0010_0000,


    Minimal    = Connection
               | Socket
               | Encryption
               | Parser,

    Full       = Connection
               | Parser    
               | Socket    
               | Xmpp      
               | State     
               | Encryption
}

#pragma warning restore format

public sealed class LogEntry
{
    public XmppLogScope Scope { get; init; }
    public string? Message { get; init; }
    public Exception? Exception { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}

public delegate void LogEventHandler(object sender, LogEntry e);