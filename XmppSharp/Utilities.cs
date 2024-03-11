namespace XmppSharp;

public static class Utilities
{
    public static bool TryUnwrap<T>(this T? self, out T result) where T : struct
    {
        result = self ?? default;
        return self.HasValue;
    }
}
