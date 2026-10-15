using Jabber.Protocol.Extensions.XEP0045;
using System.Xml;
using Xunit.Abstractions;

namespace Jabber.Test;

public class MucExtensionTests(ITestOutputHelper Output)
{
    [Theory]
    [InlineData("foo", 1, 3, 5)]
    [InlineData("bar", 2, 4, 6)]
    public void CreateMuc_StanzasCharsSecondsAndPassword(string pwd, int maxStanzas, int maxChars, int seconds)
    {
        var ext = new Muc
        {
            Password = pwd,
            History = new()
            {
                MaxStanzas = maxStanzas,
                MaxChars = maxChars,
                Seconds = seconds
            }
        };

        var el = ext.Serialize();

        Assert.Equal(el.GetTag("password"), pwd);

        var history = el.Element("history");

        Assert.NotNull(history);

        ValidateAttribute<int?>("maxstanzas", maxStanzas);
        ValidateAttribute<int?>("maxchars", maxChars);
        ValidateAttribute<int?>("seconds", seconds);

        Output.WriteLine("XML: " + el.ToString(true));

        void ValidateAttribute<T>(string name, object? currentValue)
        {
            var attr = history.Attribute(name);

            var rawValue = (T?)(dynamic)attr!;

            Assert.Equal(currentValue, rawValue);
        }
    }

    [Fact]
    public void CreateMuc_OnlyTimestamp()
    {
        var timestamp = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var ext = new Muc
        {
            History = new History
            {
                Since = timestamp
            }
        };

        var el = ext.Serialize();

        var history = el.Element("history");

        Assert.NotNull(history);

        var value = XmlConvert.ToDateTimeOffset(history.Attribute("since")!);

        Assert.Equal(timestamp, value);

        Output.WriteLine("XML: " + el.ToString(true));
    }
}
