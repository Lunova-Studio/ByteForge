using ByteForge.Core.Interfaces;
using ByteForge.Core.Patches;
using ByteForge.Core.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using System.Dynamic;
using System.Reflection;
using System.Reflection.Emit;
using OpCodes = Mono.Cecil.Cil.OpCodes;

namespace ByteForge.Core.Makers;

public class PrefixMaker : IMethodMaker
{
    public int GetTier()
    {
        return 1000;
    }

    public void OnMake(IEnumerable<IPatch> patches, DynamicMethodDefinition dynamicMethod)
    {
        IEnumerable<PrefixPatch> prefixPatches = patches.Where(x => x is PrefixPatch).Select(x => (PrefixPatch)x);

        foreach (PrefixPatch patch in prefixPatches)
        {
            PrefixMethodWithRefArgs(dynamicMethod, patch.GetMethod());
        }
    }

    private static void PrefixMethodWithRefArgs(
     DynamicMethodDefinition dmd,
     MethodInfo targetMethod,
     bool skipReturnValue = true)
    {
        ILContext ilc = new(dmd.Definition);

        ILCursor il = new(ilc);
        il.Goto(0);

        // 获取原方法信息
        MethodBase originalMethod = dmd.OriginalMethod;
        ParameterInfo[] originalParams = originalMethod.GetParameters();
        int paramCount = originalParams.Length;

        // 验证目标方法
        if (targetMethod == null)
            throw new ArgumentNullException(nameof(targetMethod));

        ParameterInfo[] targetParams = targetMethod.GetParameters();
        if (targetParams.Length != paramCount)
            throw new ArgumentException(
                $"目标方法参数数量 ({targetParams.Length}) 与原方法 ({paramCount}) 不匹配");

        // 创建局部变量数组
        VariableDefinition[] paramVars = new VariableDefinition[paramCount];
        for (int i = 0; i < paramCount; i++)
        {
            paramVars[i] = new VariableDefinition(dmd.Definition.Parameters[i].ParameterType);
            il.Body.Variables.Add(paramVars[i]); // 添加到方法体的变量集合
        }

        // 1. 存储所有参数到局部变量
        for (int i = 0; i < paramCount; i++)
        {
            // 计算正确的参数索引（考虑 this 指针）
            int argIndex = originalMethod.IsStatic ? i : i + 1;

            // 加载参数
            ILUtils.EmitLdarg(il, argIndex);

            // 存储到局部变量
            ILUtils.EmitStloc(il, paramVars[i]);
        }

        // 2. 处理 this 指针
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

        // 3. 加载所有局部变量地址
        foreach (var local in paramVars)
        {
            ILUtils.EmitLdloca(il, local);
        }

        // 4. 调用目标方法
        if (targetMethod.IsVirtual && !targetMethod.IsFinal && !targetMethod.DeclaringType.IsValueType)
        {
            il.Emit(OpCodes.Callvirt, targetMethod);
        }
        else
        {
            il.Emit(OpCodes.Call, targetMethod);
        }

        // 5. 处理返回值（如果需要丢弃）
        if (skipReturnValue && targetMethod.ReturnType != typeof(void))
        {
            il.Emit(OpCodes.Pop);
        }

        // 6. 将修改后的值写回原参数
        for (int i = 0; i < paramCount; i++)
        {
            ILUtils.EmitLdloc(il, paramVars[i]);

            int argIndex = originalMethod.IsStatic ? i : i + 1;
            ILUtils.EmitStarg(il, argIndex);
        }
    }
}
