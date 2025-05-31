using ByteForge.Interfaces;
using System.Reflection;

namespace ByteForge.Patches;

public class RewritePatch : IPatch
{
    private MethodInfo method { get; set; }

    public RewritePatch(MethodInfo method)
    {
        this.method = method;
    }

    public MethodInfo GetMethod()
    {
        return method;
    }
}
