using ByteForge.Attributes;
using ByteForge.Interfaces;
using ByteForge.Patches;
using ByteForge.Utils;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using System.Reflection;
using OpCodes = Mono.Cecil.Cil.OpCodes;

namespace ByteForge.Makers;

public class ModifyArgumentMaker : IMethodMaker
{
    public int GetTier()
    {
        return 1000;
    }
    public void OnMake(IEnumerable<IPatch> patches, DynamicMethodDefinition dynamicMethod)
    {
        IEnumerable<ModifyArgumentPatch> modifyArgumentPatches = patches.Where(x => x is ModifyArgumentPatch).Select(x => (ModifyArgumentPatch)x);

        foreach (ModifyArgumentPatch patch in modifyArgumentPatches)
        {
            InjectWithRefArgs(dynamicMethod, patch.GetMethod());
        }
    }

    private static void InjectWithRefArgs(
        DynamicMethodDefinition dmd,
        MethodInfo targetMethod)
    {
        ILContext ilc = new(dmd.Definition);

        var il = new ILCursor(ilc);
        var originalMethod = ilc.Method;

        ModifyArgumentAttribute modifyArgumentAttribute = targetMethod.GetCustomAttribute<ModifyArgumentAttribute>()!;
        MethodInfo? modify = modifyArgumentAttribute.Type.GetMethod(modifyArgumentAttribute.Name, modifyArgumentAttribute.Parameters);

        try
        {
            // 1. 查找所有目标方法调用
            var targetCalls = new List<Instruction>();
            il.Goto(0);

            foreach (var instruction in ilc.Body.Instructions)
            {
                if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                {
                    if (instruction.Operand is MethodReference methodRef2 && AreMethodsEqual(modify, methodRef2))
                    {
                        targetCalls.Add(instruction);
                    }
                }
            }

            if (targetCalls.Count == 0)
            {
                throw new InvalidOperationException(
                    $"未找到目标方法调用: {modify}");
            }

            // 2. 处理修改器
            if (modifyArgumentAttribute.CallingIndex < 0 || modifyArgumentAttribute.CallingIndex >= targetCalls.Count)
            {
                throw new ArgumentOutOfRangeException(
                    $"调用索引超出范围: {modifyArgumentAttribute.CallingIndex} (有效范围: 0-{targetCalls.Count - 1})");
            }

            var callInstruction = targetCalls[modifyArgumentAttribute.CallingIndex];
            var methodRef = (MethodReference)callInstruction.Operand;
            int paramCount = methodRef.Parameters.Count;

            if (modifyArgumentAttribute.ArgumentIndex < 0 || modifyArgumentAttribute.ArgumentIndex >= paramCount)
            {
                throw new ArgumentOutOfRangeException(
                    $"参数索引超出范围: {modifyArgumentAttribute.ArgumentIndex} (有效范围: 0-{paramCount - 1})");
            }

            // 3. 移动到调用指令前
            il.Goto(callInstruction, MoveType.Before);

            // 4. 创建参数存储变量
            var paramVars = new VariableDefinition[paramCount];
            for (int i = 0; i < paramCount; i++)
            {
                paramVars[i] = new VariableDefinition(methodRef.Parameters[i].ParameterType);
                ilc.Body.Variables.Add(paramVars[i]);
            }

            // 5. 存储所有参数到局部变量
            for (int i = paramCount - 1; i >= 0; i--)
            {
                il.Emit(OpCodes.Stloc, paramVars[i]);
            }

            // 处理 this 指针
            bool isTargetInstance = !targetMethod.IsStatic;
            bool isOriginalInstance = !originalMethod.IsStatic;

            if (isTargetInstance && isOriginalInstance)
            {
                // 加载 this 指针
                ILUtils.EmitLdarg(il, 0);
            }
            else if (isTargetInstance && !isOriginalInstance)
            {
                throw new InvalidOperationException(
                    "无法在静态方法中调用实例方法：缺少 this 指针");
            }

            // 6. 修改目标参数
            il.Goto(callInstruction, MoveType.Before);

            // 加载要修改的参数值
            il.Emit(OpCodes.Ldloca, paramVars[modifyArgumentAttribute.ArgumentIndex]);

            // 调用修改方法
            il.Emit(OpCodes.Call, targetMethod);

            // 7. 重新加载所有参数
            il.Goto(callInstruction, MoveType.Before);
            foreach (var local in paramVars)
            {
                il.Emit(OpCodes.Ldloc, local);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"修改方法参数失败: {ex.Message}", ex);
        }
    }

    public static bool AreMethodsEqual(
        MethodInfo reflectionMethod,
        MethodReference cecilMethod)
    {
        if (reflectionMethod == null || cecilMethod == null)
            return false;

        // 1. 比较基本属性
        if (reflectionMethod.Name != cecilMethod.Name)
            return false;

        if (reflectionMethod.IsStatic != cecilMethod.Resolve().IsStatic)
            return false;

        // 2. 比较声明类型
        if (!AreTypesEqual(reflectionMethod.DeclaringType, cecilMethod.DeclaringType))
            return false;

        // 3. 比较返回类型
        if (!AreTypesEqual(reflectionMethod.ReturnType, cecilMethod.ReturnType))
            return false;

        // 4. 比较参数
        var reflectionParams = reflectionMethod.GetParameters();
        var cecilParams = cecilMethod.Parameters;

        if (reflectionParams.Length != cecilParams.Count)
            return false;

        for (int i = 0; i < reflectionParams.Length; i++)
        {
            if (!AreTypesEqual(reflectionParams[i].ParameterType, cecilParams[i].ParameterType))
                return false;
        }

        // 5. 比较泛型参数
        if (reflectionMethod.IsGenericMethod != cecilMethod.IsGenericInstance)
            return false;

        return true;
    }

    // 辅助方法：比较类型是否相等
    public static bool AreTypesEqual(Type reflectionType, TypeReference cecilType)
    {
        if (reflectionType == null || cecilType == null)
            return false;

        // 处理泛型类型
        if (reflectionType.IsGenericType && cecilType.IsGenericInstance)
        {
            var reflectionGeneric = reflectionType.GetGenericTypeDefinition();
            var cecilGeneric = ((GenericInstanceType)cecilType).ElementType;

            if (reflectionGeneric.FullName != cecilGeneric.FullName)
                return false;

            var reflectionArgs = reflectionType.GetGenericArguments();
            var cecilArgs = ((GenericInstanceType)cecilType).GenericArguments;

            if (reflectionArgs.Length != cecilArgs.Count)
                return false;

            for (int i = 0; i < reflectionArgs.Length; i++)
            {
                if (!AreTypesEqual(reflectionArgs[i], cecilArgs[i]))
                    return false;
            }

            return true;
        }

        // 处理数组类型
        if (reflectionType.IsArray && cecilType.IsArray)
        {
            var reflectionElement = reflectionType.GetElementType();
            var cecilElement = ((ArrayType)cecilType).ElementType;
            return AreTypesEqual(reflectionElement, cecilElement);
        }

        // 处理普通类型
        return NormalizeTypeName(reflectionType) == NormalizeTypeName(cecilType);
    }

    // 标准化类型名称（解决不同命名约定问题）
    private static string NormalizeTypeName(Type type)
    {
        if (type == null) return null;

        // 处理嵌套类型
        if (type.IsNested)
        {
            return $"{type.DeclaringType.FullName}/{type.Name}";
        }

        // 处理泛型类型定义
        if (type.IsGenericType && !type.IsGenericTypeDefinition)
        {
            return type.GetGenericTypeDefinition().FullName;
        }

        return type.FullName;
    }

    private static string NormalizeTypeName(TypeReference typeRef)
    {
        if (typeRef == null) return null;

        // 处理嵌套类型
        if (typeRef.IsNested)
        {
            return $"{typeRef.DeclaringType.FullName}/{typeRef.Name}";
        }

        // 处理泛型类型定义
        if (typeRef.IsGenericInstance)
        {
            return ((GenericInstanceType)typeRef).ElementType.FullName;
        }

        return typeRef.FullName;
    }
}
