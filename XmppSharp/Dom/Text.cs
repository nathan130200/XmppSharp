using System.Diagnostics;

namespace XmppSharp.Dom;

[DebuggerDisplay("Text: {Value,nq}")]
public class Text : Node
{
    public string? Value
    {
        get;
        set;
    }

    public Text()
    {

    }

    public Text(string? value)
    {
        Value = value;
    }

    public override string ToString()
        => Value ?? string.Empty;

    public override Node Clone()
        => new Text(Value);
}

// ---------------------------------------------------- //

[DebuggerDisplay("Comment: {Value,nq}")]
public class Comment : Node
{
    public string? Value
    {
        get;
        set;
    }

    public Comment()
    {

    }

    public Comment(string? value)
    {
        Value = value;
    }

    public override string ToString()
        => Value ?? string.Empty;

    public override Node Clone()
        => new Comment(Value);
}

// ---------------------------------------------------- //

[DebuggerDisplay("Cdata: {Value,nq}")]
public class Cdata : Node
{
    public string? Value
    {
        get;
        set;
    }

    public Cdata()
    {

    }

    public Cdata(string? value)
    {
        Value = value;
    }

    public override string ToString()
        => Value ?? string.Empty;

    public override Node Clone()
        => new Cdata(Value);
}