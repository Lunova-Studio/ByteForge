using ByteForge.Interfaces;
using ByteForge.Patches;
using ByteForge.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using System.Reflection;

namespace ByteForge.Makers;

public class PostfixMaker : IMethodMaker
{
    public int GetTier()
    {
        return 1000;
    }

    public void OnMake(IEnumerable<IPatch> patches, DynamicMethodDefinition dynamicMethod)
    {
        IEnumerable<PostfixPatch> prefixPatches = patches.Where(x => x is PostfixPatch).Select(x => (PostfixPatch)x);

        foreach (PostfixPatch patch in prefixPatches)
        {
            PostfixMethodWithRefArgs(dynamicMethod, patch.GetMethod());
        }
    }

    public static void PostfixMethodWithRefArgs(
        DynamicMethodDefinition dmd,
        MethodInfo targetMethod,
        bool includeReturnValue = true)
    {
        ILContext ilc = new(dmd.Definition);
        var il = new ILCursor(ilc);
        var originalMethod = ilc.Method;
        var parameters = originalMethod.Parameters;
        int paramCount = parameters.Count;
        bool isInstanceMethod = originalMethod.HasThis;
        bool hasReturnValue = originalMethod.ReturnType.FullName != "System.Void";

        // 1. 声明局部变量数组（用于存储参数）
        VariableDefinition[] paramVars = new VariableDefinition[paramCount];
        for (int i = 0; i < paramCount; i++)
        {
            paramVars[i] = new VariableDefinition(parameters[i].ParameterType);
            ilc.Body.Variables.Add(paramVars[i]);
        }

        // 2. 声明返回值变量（如果需要）
        VariableDefinition returnVar = null;
        if (includeReturnValue && hasReturnValue)
        {
            returnVar = new VariableDefinition(originalMethod.ReturnType);
            ilc.Body.Variables.Add(returnVar);
        }

        // 3. 找到所有返回指令（ret）
        var retInstructions = ilc.Instrs
            .Where(i => i.OpCode == OpCodes.Ret)
            .ToList();

        if (retInstructions.Count == 0)
            throw new InvalidOperationException("方法中没有找到返回指令");

        // 4. 在每个返回指令前插入 Postfix 逻辑
        foreach (var retInstr in retInstructions)
        {
            il.Goto(retInstr, MoveType.Before);

            // 保存当前栈状态（如果有返回值）
            if (includeReturnValue && hasReturnValue)
            {
                // 存储返回值到局部变量
                il.Emit(OpCodes.Stloc, returnVar);
            }

            // 5. 存储所有参数到局部变量（使用当前值）
            for (int i = 0; i < paramCount; i++)
            {
                il.Emit(OpCodes.Ldarg, parameters[i]);
                il.Emit(OpCodes.Stloc, paramVars[i]);
            }

            // 6. 处理 this 指针（如果是实例方法）
            if (isInstanceMethod)
            {
                il.Emit(OpCodes.Ldarg_0);
            }

            // 7. 加载所有局部变量地址（ref 参数）
            foreach (var local in paramVars)
            {
                ILUtils.EmitLdloca(il, local);
            }

            // 8. 加载返回值地址（如果需要）
            if (includeReturnValue && hasReturnValue)
            {
                ILUtils.EmitLdloca(il, returnVar);
            }

            // 9. 调用目标方法
            il.Emit(!targetMethod.IsStatic ? OpCodes.Callvirt : OpCodes.Call, targetMethod);

            // 10. 将修改后的值写回原参数
            for (int i = 0; i < paramCount; i++)
            {
                // 跳过 this 指针（它不可修改）
                if (i == 0 && isInstanceMethod) continue;

                il.Emit(OpCodes.Ldloc, paramVars[i]);
                il.Emit(OpCodes.Starg, parameters[i]);
            }

            // 11. 恢复返回值（如果需要）
            if (includeReturnValue && hasReturnValue)
            {
                il.Emit(OpCodes.Ldloc, returnVar);
            }
        }
    }


}
