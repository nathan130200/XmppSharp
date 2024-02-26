namespace XmppSharp.Utilities;

internal static class XmppEnumUtil
{
	internal static bool EqualityComparer<T>(T self, object? obj) where T : IXmppEnum
	{
		if (obj is not T other)
			return false;

		if (!self.HasValue || !other.HasValue)
			return false;

		return string.Equals(self.Value, other.Value);
	}
}
