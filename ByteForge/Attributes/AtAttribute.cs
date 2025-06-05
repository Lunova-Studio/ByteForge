using ByteForge.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ByteForge.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AtAttribute : Attribute, IEquatable<AtAttribute?>
{
    public string Name { get; }
    public Type? ReturnType { get; }
    public Type[] Parameters { get; }

    public AtAttribute(string name, Type? returnType, params Type[] parameters)
    {
        Name = name;
        ReturnType = returnType;
        Parameters = parameters;
    }

    public virtual MethodInfo GetTarget(Type mixinType)
    {
        MixinAttribute mixinAttribute = mixinType.GetCustomAttribute<MixinAttribute>()!;
        MethodInfo? target = mixinAttribute.Target.GetMethod(Name, Parameters);
        if (target == null) throw new TargetMethodNotFoundException(null, this, mixinAttribute.Target);

        return target;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj.GetType() != GetType()) return false;
        AtAttribute at = obj as AtAttribute;

        return Name == at.Name && ReturnType == at.ReturnType && Parameters.SequenceEqual(at.Parameters);
    }

    public bool Equals(AtAttribute? other)
    {
        return Equals((object?)other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Name, ReturnType, Parameters);
    }

    public static bool operator ==(AtAttribute? lhs, AtAttribute? rhs)
    {
        if (lhs is null && rhs is null) return true;
        if (lhs is not null) return lhs.Equals(rhs);
        return false;
    }

    public static bool operator !=(AtAttribute? lhs, AtAttribute? rhs)
    {
        return !(lhs == rhs);
    }
}
