using ByteForge.Attributes;
using ByteForge.Exceptions;
using System.Reflection;

namespace ByteForge.Interfaces;

public interface IPatch
{
    public MethodInfo GetMethod();
}
