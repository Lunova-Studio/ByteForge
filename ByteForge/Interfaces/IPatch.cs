using ByteForge.Attributes;
using ByteForge.Exceptions;
using System.Reflection;

namespace ByteForge.Interfaces;

public interface IPatch
{
    public MethodInfo GetMethod();
    static MethodInfo GetTarget(IPatch patch)
    {
        AtAttribute? at = patch.GetMethod().GetCustomAttribute<AtAttribute>();
        if (at == null) throw new InvaildAtAttributeException(patch, at);
        MixinAttribute mixinAttribute = patch.GetMethod().DeclaringType.GetCustomAttribute<MixinAttribute>()!;
        MethodInfo? target = mixinAttribute.Target.GetMethod(at.Name, at.Parameters);
        if (target == null) throw new TargetMethodNotFoundException(patch, at, mixinAttribute.Target);

        return target;
    }
}
