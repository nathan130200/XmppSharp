using XmppSharp.Factory;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

/// <summary>
/// Base class to implement an XMPP parser.
/// </summary>
public abstract class XmppParser : IDisposable
{
	/// <summary>
	/// The event is triggered when the XMPP open tag is found <c>&lt;stream:stream&gt;</c>
	/// </summary>
	public event AsyncAction<StreamStream> OnStreamStart;

	/// <summary>
	/// The event is triggered when any well-formed element is found.
	/// <para>However, if the XML tag is registered using <see cref="ElementFactory" /> the parser will automatically construct the element in the registered type.</para>
	/// <para>Elements that cannot be constructed using <see cref="ElementFactory" /> only return element as base type of <see cref="Element" />.</para>
	/// </summary>
	public event AsyncAction<Element> OnStreamElement;

	/// <summary>
	/// The event is triggered when the XMPP close tag is found <c>&lt;/stream:stream&gt;</c>
	/// </summary>
	public event AsyncAction OnStreamEnd;

	private volatile bool _disposed;

	protected bool IsDisposed
	{
		get => _disposed;
		set => _disposed = value;
	}

	protected void ThrowIfDisposed()
		=> ObjectDisposedException.ThrowIf(_disposed, this);

	/// <summary>
	/// Method that is called when disposing the parser.
	/// </summary>
	protected virtual void Release()
	{

	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			Release();
			GC.SuppressFinalize(this);
		}
	}

	protected async Task FireStreamStart(StreamStream e)
	{
		await Task.Yield();

		if (OnStreamStart != null)
			await OnStreamStart(e);
	}

	protected async Task FireStreamEnd()
	{
		await Task.Yield();

		if (OnStreamEnd != null)
			await OnStreamEnd();
	}

	protected async Task FireStreamElement(Element e)
	{
		await Task.Yield();

		if (OnStreamElement != null)
			await OnStreamElement(e);
	}
}
