using ByteForge.Attributes;
using ByteForge.Interfaces;
using ByteForge.Patches;
using ByteForge.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ByteForge.Factories;

public class ModifyArgumentFactory : IPatchFactory
{
    public bool IsValid(IPatch patch)
    {
        MethodInfo target = IPatchUtils.GetTarget(patch);
        MethodInfo method = patch.GetMethod();

        ModifyArgumentAttribute modifyArgumentAttribute = method.GetCustomAttribute<ModifyArgumentAttribute>()!;
        MethodInfo? modify = modifyArgumentAttribute.Type.GetMethod(modifyArgumentAttribute.Name, modifyArgumentAttribute.Parameters);

        return method.ReturnType == typeof(void) &&
               method.GetParameters().Length == 1 &&
               method.GetParameters()[0].ParameterType == modify.GetParameters()[modifyArgumentAttribute.ArgumentIndex].ParameterType.MakeByRefType() &&
               target.IsStatic == method.IsStatic;
    }

    public IEnumerable<IPatch> SearchAll(Type mixin)
    {
        return mixin.GetMethods().Where(x => x.GetCustomAttribute<ModifyArgumentAttribute>() != null).Select(x => new ModifyArgumentPatch(x));
    }
}
