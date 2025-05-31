using MonoMod.Utils;
using MonoMod.Utils.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteForge.Core.Interfaces;

public interface IMethodMaker
{
    public int GetTier();
    public void OnMake(IEnumerable<IPatch> patches, DynamicMethodDefinition dynamicMethod);
}
