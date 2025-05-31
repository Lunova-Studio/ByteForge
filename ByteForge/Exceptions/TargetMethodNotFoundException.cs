using ByteForge.Attributes;
using ByteForge.Interfaces;

namespace ByteForge.Exceptions;

public class TargetMethodNotFoundException : Exception
{
    public IPatch Patch { get; }
    public AtAttribute At { get; }
    public Type TargetType { get; }

    public TargetMethodNotFoundException(IPatch patch, AtAttribute at, Type targetType)
        : base($"Target method '{at.Name}' with specified parameters not found in type '{targetType.FullName}'.")
    {
        Patch = patch;
        At = at;
        TargetType = targetType;
    }
}
