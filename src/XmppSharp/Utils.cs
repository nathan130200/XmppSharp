namespace XmppSharp;

public static class Utils
{
    public static bool TryUnwrap<S>(this S? value, out S result) where S : struct
    {
        result = value ?? default;
        return value.HasValue;
    }
}

