using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using XmppSharp.Parser;

namespace XmppSharp;

public static class Utilities
{
	public static ReadOnlyJid AsReadOnly(this Jid jid)
		=> new(jid);

	public static bool TryGetValue<T>(this T? self, out T result) where T : struct
	{
		result = self ?? default;
		return self.HasValue;
	}

	public static string GetString(this byte[] buffer, Encoding? encoding = default)
		=> (encoding ?? Encoding.UTF8).GetString(buffer);

	public static byte[] GetBytes(this string s, Encoding? encoding = default)
		=> (encoding ?? Encoding.UTF8).GetBytes(s);

	public static string ToHex(this byte[] bytes, bool lowercase = true)
	{
		var result = Convert.ToHexString(bytes);

		if (!lowercase)
			return result;

		return result.ToLowerInvariant();
	}

	public static async Task<Element> GetNextElementAsync(this XmppStreamParser parser, CancellationToken token = default)
	{
		var tcs = new TaskCompletionSource<Element>();

		using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

		AsyncAction<Element> handler = default;

		handler = element =>
		{
			cts.Cancel();
			tcs.TrySetResult(element);
			parser.OnStreamElement -= handler;
			return Task.CompletedTask;
		};

		_ = Task.Run(async () =>
		{
			try
			{
				while (!cts.IsCancellationRequested)
				{
					if (!await parser)
						throw new XmlException("Unexcepted end of stream.");
				}
			}
			catch (Exception e)
			{
				tcs.TrySetException(e);
			}
			finally
			{
				// ensure we always unbind the handler.
				parser.OnStreamElement -= handler;
			}
		});

		parser.OnStreamElement += handler;

		return await tcs.Task;
	}

	public static TaskAwaiter<bool> GetAwaiter(this XmppStreamParser parser)
		=> parser.AdvanceAsync().GetAwaiter();

	public static byte[] FromHex(this string str)
		=> Convert.FromHexString(str);

	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> callback)
	{
		Require.NotNull(source);
		Require.NotNull(callback);

		foreach (var item in source)
			callback(item);

		return source;
	}

	public static bool TryGetChild(this Element e, string tagName, string? namespaceURI, out Element result)
	{
		result = e.Child(tagName, namespaceURI);
		return result != null;
	}

	public static bool TryGetChild(this Element e, string tagName, out Element result)
	{
		result = e.Child(tagName);
		return result != null;
	}
}
