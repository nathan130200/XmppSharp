using System.Web;

namespace XmppSharp
{
    public static class Util
    {
        public static string EscapeXml(string str)
            => HttpUtility.HtmlEncode(str);

        public static string UnescapeXml(string str)
            => HttpUtility.HtmlDecode(str);
    }
}
