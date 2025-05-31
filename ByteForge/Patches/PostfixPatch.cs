using ByteForge.Interfaces;
using System.Reflection;

namespace ByteForge.Patches;

public class PostfixPatch : IPatch
{
    private MethodInfo method { get; set; }

    public PostfixPatch(MethodInfo method)
    {
        this.method = method;
    }

    public MethodInfo GetMethod()
    {
        return method;
    }
}
