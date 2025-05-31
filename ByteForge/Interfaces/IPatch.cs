using System.Reflection;

namespace ByteForge.Interfaces;

public interface IPatch
{
    public MethodInfo GetMethod();
}
