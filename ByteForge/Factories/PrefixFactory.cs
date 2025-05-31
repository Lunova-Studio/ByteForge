using ByteForge.Attributes;
using ByteForge.Patches;
using ByteForge.Interfaces;
using System.Reflection;

namespace ByteForge.Factories;

public class PrefixFactory : IPatchFactory
{
    public IEnumerable<IPatch> SearchAll(Type mixin)
    {
        return mixin.GetMethods().Where(x => x.GetCustomAttribute<PrefixAttribute>() != null).Select(x => new PrefixPatch(x));
    }
}
