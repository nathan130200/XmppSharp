using System.Diagnostics;

namespace XmppSharp.Dom;

public abstract class Node : ICloneable
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal Element? _parent;

    public abstract Node Clone();

    object ICloneable.Clone() => Clone();

    public Element? Parent
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
