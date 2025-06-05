using ByteForge.Attributes;
using ByteForge.Exceptions;
using ByteForge.Interfaces;
using System.Reflection;

namespace ByteForge.Utils;

public static class IPatchUtils
{
    public static MethodInfo GetTarget(IPatch patch)
    {
        AtAttribute? at = patch.GetMethod().GetCustomAttribute<AtAttribute>();
        if (at == null) throw new InvaildAtAttributeException(patch, at);

        return at.GetTarget(patch.GetMethod().DeclaringType!);
    }
}