using System.Diagnostics;
using System.Xml.Linq;

namespace XmppSharp.Test;

[TestClass]
public class ElementTest
{
    [TestMethod]
    public void UpdateNamespaceRecursive()
    {
        var root = new XElement("{urn:xmppsharp:testing}root");

        for (int i = 0; i < 10; i++)
        {
            var row = new XElement($"{{urn:row}}row_{i}");

            for (int j = 0; j < 10; j++)
                row.Add(new XElement($"cell_{j}"));

            root.Add(row);
        }

        Debug.WriteLine(root.ToString(SaveOptions.OmitDuplicateNamespaces));

        root.SetNamespace(Namespace.Client);

        Debug.WriteLine("New Namespace: " + root.GetNamespace());

        Debug.WriteLine(root.ToString(SaveOptions.OmitDuplicateNamespaces));
    }

    [TestMethod]
    public void GetNamespace()
    {
        var el = new XElement("query");
        var name = el.Name;

        Assert.AreEqual(name.LocalName, "query");
        Assert.AreEqual(XNamespace.None, name.Namespace);
        Debug.WriteLine("[Before] Namespace: " + el.GetAttribute("xmlns"));
        Assert.IsNull(el.GetAttribute("xmlns"));

        el.SetAttribute("xmlns", "urn:cryonline:k01");

        name = el.Name = string.Concat('{', el.GetAttribute("xmlns"), '}', name.LocalName);

        Assert.AreEqual(name.LocalName, "query");
        Assert.IsNotNull(el.GetAttribute("xmlns"));
        Debug.WriteLine("[After] Namespace: " + el.GetAttribute("xmlns"));
        Assert.AreEqual(Namespace.CryOnline, name.Namespace);
    }
}
