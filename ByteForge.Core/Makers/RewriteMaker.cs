using ByteForge.Core.Interfaces;
using ByteForge.Core.Patches;
using Mono.Cecil.Cil;
using MonoMod.Utils;
using MonoMod.Utils.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ByteForge.Core.Makers;

public class RewriteMaker : IMethodMaker
{
    public int GetTier()
    {
        return 1000;
    }

    public void OnMake(IEnumerable<IPatch> patches, DynamicMethodDefinition dynamicMethod)
    {
        IEnumerable<RewritePatch> rewritePatches = patches.Where(x => x is RewritePatch).Select(x => (RewritePatch)x);
        if (rewritePatches.Any())
        {
            if (rewritePatches.Count() > 1) throw new ArgumentException("最多只接受一个");

            RewritePatch patch = rewritePatches.First();

            DynamicMethodDefinition dmd = new DynamicMethodDefinition(patch.GetMethod());

            ILProcessor result = dynamicMethod.GetILProcessor();
            ILProcessor il = dmd.GetILProcessor();

            result.Clear();
            foreach (Instruction instruction in il.Body.Instructions)
            {
                result.Append(instruction);
            }
        }
    }
}
