using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace XmppSharp;

[StackTraceHidden]
static class Require
{
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
