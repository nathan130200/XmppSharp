using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jabber;

#pragma warning disable

// An work-around to automatically fill all struct enums for faster caching & access.

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
internal class RunStaticCtorAttribute : Attribute
{
    [ModuleInitializer]
    internal static void InvokeStaticCtors()
    {
        foreach (var type in typeof(RunStaticCtorAttribute).Assembly.GetTypes())
        {
            if (type.GetCustomAttributes<RunStaticCtorAttribute>().Any())
            {
                try
                {
                    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}
#pragma warning restore