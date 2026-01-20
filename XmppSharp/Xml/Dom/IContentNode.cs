using System.Diagnostics.CodeAnalysis;

namespace XmppSharp.Dom;

public interface IContentNode
{
	[MaybeNull]
	string Value { get; }
}
