using System.ComponentModel;
using System.Diagnostics;

namespace XmppSharp.Dom;

[DebuggerDisplay("{Value,nq}")]
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class ContentNode : Node
{

}
