using System.Diagnostics;
using XmppSharp;

internal class Program
{
    private static void Main(string[] args)
    {
        Jid bare1 = "foo@bar";
        Jid bare2 = "foo@bar";

        var isBareEquals = bare1 == bare2;
        Debug.Assert(isBareEquals);

        Jid full1 = "foo@bar/baz";
        Jid full2 = "foo@bar/baz";

        var isFullEquals = full1 == full2;
        Debug.Assert(isFullEquals);

        bare1.Domain = "baz";
        var isNotBareEquals = bare1 == bare2;
        Debug.Assert(isNotBareEquals == false);

        full2.Resource = "res";
        var isNotFullEquals = full1 == full2;
        Debug.Assert(isNotFullEquals == false);
    }
}