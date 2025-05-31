using ByteForge.Attributes;
using ByteForge.Interfaces;
using ByteForge.Patches;
using System.Reflection;

namespace ByteForge.Factories;

public class PostfixFactory : IPatchFactory
{
    public bool IsValid(IPatch patch)
    {
        MethodInfo target = IPatch.GetTarget(patch);
        MethodInfo method = patch.GetMethod();

        return method.ReturnType == typeof(void) &&
               target.GetParameters().Length + 1 == method.GetParameters().Length &&
               target.GetParameters().Select(x => x.ParameterType).Append(target.ReturnType).Where(x => x != typeof(void)).Select(x => x.MakeByRefType()).SequenceEqual(method.GetParameters().Select(x => x.ParameterType)) &&
               target.IsStatic == method.IsStatic;
    }

    public IEnumerable<IPatch> SearchAll(Type mixin)
    {
        return mixin.GetMethods().Where(x => x.GetCustomAttribute<PostfixAttribute>() != null).Select(x => new PostfixPatch(x));
    }
}
