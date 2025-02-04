using System.Diagnostics;

namespace XmppSharp.Dom;

[DebuggerDisplay("Text: {Value,nq}")]
public class XmppText : XmppNode
{
    public string? Value
    {
        get;
        set;
    }

    public XmppText()
    {

    }

    public XmppText(string? value)
    {
        Value = value;
    }

    public override string ToString()
        => Value ?? string.Empty;

    public override XmppNode Clone()
        => new XmppText(Value);
}

// ---------------------------------------------------- //

[DebuggerDisplay("Comment: {Value,nq}")]
public class XmppComment : XmppNode
{
    public string? Value
    {
        get;
        set;
    }

    public XmppComment()
    {

    }

    public XmppComment(string? value)
    {
        Value = value;
    }

    public override string ToString()
        => $"<!--{Value}-->";

    public override XmppNode Clone()
        => new XmppComment(Value);
}

// ---------------------------------------------------- //

[DebuggerDisplay("Cdata: {Value,nq}")]
public class XmppCdata : XmppNode
{
    public string? Value
    {
        get;
        set;
    }

    public XmppCdata()
    {

    }

    public XmppCdata(string? value)
    {
        Value = value;
    }

    public override string ToString()
        => $"<![CDATA[{Value}]]>";

    public override XmppNode Clone()
        => new XmppCdata(Value);
}