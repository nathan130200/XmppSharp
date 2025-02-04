namespace XmppSharp;

public class JabberException : Exception
{
    public JabberException() : base()
    {
    }

    public JabberException(string? message) : base(message)
    {
    }

    public JabberException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}