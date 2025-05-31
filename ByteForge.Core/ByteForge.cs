using ByteForge.Core.Attributes;
using ByteForge.Core.Exceptions;
using ByteForge.Core.Factories;
using ByteForge.Core.Interfaces;
using ByteForge.Core.Makers;
using ByteForge.Core.Utils;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Core;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ByteForge.Core;

public class ByteForge
{
    public List<IMethodMaker> MethodMakers { get; private set; } = new();
    public List<IPatchFactory> PatchFactories { get; private set; } = new();
    private List<Type> mixins = new();
    public ByteForge()
    {
        MethodMakers.Add(new RewriteMaker());
        PatchFactories.Add(new RewriteFactory());
        MethodMakers.Add(new PrefixMaker());
        PatchFactories.Add(new PrefixFactory());
        MethodMakers.Add(new PostfixMaker());
        PatchFactories.Add(new PostfixFactory());
    }

    public void LoadMixin<T>()
    {
        LoadMixin(typeof(T));
    }

    public void LoadMixin(Type mixinType)
    {
        mixins.Add(mixinType);
        Apply(mixinType);
    }

    private void Apply(Type mixinType)
    {
        List<IPatch> patches = new();
        MixinAttribute mixinAttribute = mixinType.GetCustomAttribute<MixinAttribute>()!;

        foreach (IPatchFactory factory in PatchFactories)
        {
            patches.AddRange(factory.SearchAll(mixinType));
        }

        Dictionary<AtAttribute, DynamicMethodDefinition> dynamics = new();
        foreach (IPatch patch in patches)
        {
            AtAttribute? at = patch.GetMethod().GetCustomAttribute<AtAttribute>();
            if (at == null || dynamics.ContainsKey(at)) continue;
            dynamics[at] = new DynamicMethodDefinition(mixinAttribute.Target.GetMethod(at.Name, at.Parameters));
        }
        foreach (AtAttribute atAttribute in dynamics.Keys)
        {
            List<IPatch> tmp = new List<IPatch>();
            foreach (IPatch patch in patches)
            {
                AtAttribute? at = patch.GetMethod().GetCustomAttribute<AtAttribute>();
                if (atAttribute == at) tmp.Add(patch);
            }

            DynamicMethodDefinition dynamicMethod = dynamics[atAttribute];
            foreach (IMethodMaker methodMaker in MethodMakers)
            {
                methodMaker.OnMake(tmp, dynamicMethod);
            }

            MethodInfo @new = dynamicMethod.Generate();

            foreach(Instruction instruction in dynamicMethod.Definition.Body.Instructions)
            {
                Console.WriteLine(instruction);
            }
            Console.WriteLine("           ");

            DetourFactory.Current.CreateDetour(mixinAttribute.Target.GetMethod(atAttribute.Name, atAttribute.Parameters), @new);
        }
    }
}
