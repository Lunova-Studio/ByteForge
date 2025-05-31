using ByteForge.Interfaces;
using System.Reflection;

namespace ByteForge.Patches;

public class PrefixPatch : IPatch
{
    private MethodInfo method { get; set; }

    public PrefixPatch(MethodInfo method)
    {
        this.method = method;
    }

    public MethodInfo GetMethod()
    {
        return method;
    }
}
