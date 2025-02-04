using System.Globalization;
using XmppSharp.Dom;

namespace XmppSharp.Collections;

public static class XmppElementExtensions
{
    internal delegate bool OutTryParseNumberDelegate<T>(
        string? s,
        NumberStyles style,
        IFormatProvider? ifp,
        out T result);

    public delegate (bool Success, T Result) TryParseNumberDelegate<T>(
        string? s,
        NumberStyles style,
        IFormatProvider? formatProvider);

    static TryParseNumberDelegate<T> CreateNumberParser<T>(OutTryParseNumberDelegate<T> func)
        => (s, t, f) => (func(s, t, f, out var r), r);

    public static readonly TryParseNumberDelegate<int> Int32Parser = CreateNumberParser<int>(int.TryParse);
    public static readonly TryParseNumberDelegate<long> Int64Parser = CreateNumberParser<long>(long.TryParse);
    public static readonly TryParseNumberDelegate<float> FloatParser = CreateNumberParser<float>(float.TryParse);
    public static readonly TryParseNumberDelegate<double> DoubleParser = CreateNumberParser<double>(double.TryParse);

    public static int GetInt32(this XmppElement e, string name, int defaultValue = default, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = Int32Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static long GetInt64(this XmppElement e, string name, long defaultValue = default, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = Int64Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static float GetFloat(this XmppElement e, string name, float defaultValue = default, NumberStyles style = NumberStyles.Float, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = FloatParser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static double GetDouble(this XmppElement e, string name, double defaultValue = default, NumberStyles style = NumberStyles.Float, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = DoubleParser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static Jid? GetJid(this XmppElement e, string name, Jid? defaultValue = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        return temp != null ? temp : defaultValue;
    }
}