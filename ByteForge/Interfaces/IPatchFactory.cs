namespace ByteForge.Interfaces;

public interface IPatchFactory
{
    public IEnumerable<IPatch> SearchAll(Type mixin);
    public bool IsValid(IPatch patch);
}
