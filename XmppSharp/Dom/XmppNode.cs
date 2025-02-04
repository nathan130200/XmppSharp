using System.Diagnostics;

namespace XmppSharp.Dom;

public abstract class XmppNode : ICloneable
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal XmppElement? _parent;

    public abstract XmppNode Clone();

    object ICloneable.Clone() => Clone();

    public XmppElement? Parent
    {
        get => _parent;
        set
        {
            _parent?.RemoveChild(this);
            value?.AddChild(this);
        }
    }

    public void Remove()
        => _parent?.RemoveChild(this);
}
