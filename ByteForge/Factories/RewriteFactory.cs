using ByteForge.Attributes;
using ByteForge.Patches;
using ByteForge.Interfaces;
using System.Reflection;

namespace ByteForge.Factories;

public class RewriteFactory : IPatchFactory
{
    public IEnumerable<IPatch> SearchAll(Type mixin)
    {
        return mixin.GetMethods().Where(x => x.GetCustomAttribute<RewriteAttribute>() != null).Select(x => new RewritePatch(x));
    }
}
