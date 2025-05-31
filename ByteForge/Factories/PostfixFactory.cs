using ByteForge.Attributes;
using ByteForge.Patches;
using ByteForge.Interfaces;
using System.Reflection;

namespace ByteForge.Factories;

public class PostfixFactory : IPatchFactory
{
    public IEnumerable<IPatch> SearchAll(Type mixin)
    {
        return mixin.GetMethods().Where(x => x.GetCustomAttribute<PostfixAttribute>() != null).Select(x => new PostfixPatch(x));
    }
}
