namespace ByteForge.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MixinAttribute : Attribute
{
    public Type Target { get; }
    public MixinAttribute(Type target)
    {
        Target = target;
    }
}
