using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace XmppSharp;

[StackTraceHidden]
static class Require
{
	public static void NotDisposed(bool condition, object value)
	{
		NotNull(value);

#if NET7_0_OR_GREATER
		ObjectDisposedException.ThrowIf(condition, value);
#else
	if (condition)
		throw new ObjectDisposedException(value.GetType().FullName);
#endif
	}

	public static void NotDisposed(bool condition, Type type)
	{
		NotNull(type);

#if NET7_0_OR_GREATER
		ObjectDisposedException.ThrowIf(condition, type);
#else
	if (condition)
		throw new ObjectDisposedException(type.FullName);
#endif
	}

	public static void NotNull(object value, [CallerArgumentExpression(nameof(value))] string expression = default)
	{
		if (value is null)
			throw new ArgumentNullException(expression);
	}

	public static void NotNullOrEmpty(string s, [CallerArgumentExpression(nameof(s))] string expression = default)
	{
		if (string.IsNullOrEmpty(s))
			throw new ArgumentNullException(expression);
	}

	public static void NotNullOrWhiteSpace(string s, [CallerArgumentExpression(nameof(s))] string expression = default)
	{
		if (string.IsNullOrEmpty(s))
			throw new ArgumentNullException(expression);
	}
}
