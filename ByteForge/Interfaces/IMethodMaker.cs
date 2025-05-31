using MonoMod.Utils;

namespace ByteForge.Interfaces;

public interface IMethodMaker
{
    public int GetTier();
    public void OnMake(IEnumerable<IPatch> patches, DynamicMethodDefinition dynamicMethod);
}
