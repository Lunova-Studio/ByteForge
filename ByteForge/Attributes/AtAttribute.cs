using System.Diagnostics.CodeAnalysis;

namespace ByteForge.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AtAttribute : Attribute, IEquatable<AtAttribute?>
{
    public string Name { get; set; }
    public Type? ReturnType { get; set; }
    public Type[] Parameters { get; set; }

    public AtAttribute(string name, Type? returnType, params Type[] palarmters)
    {
        Name = name;
        ReturnType = returnType;
        Parameters = palarmters;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
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
