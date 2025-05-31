using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace ByteForge.Utils;

public static class ILUtils
{
    // 加载参数
    public static void EmitLdarg(ILCursor il, int index)
    {
        switch (index)
        {
            case 0:
                il.Emit(OpCodes.Ldarg_0);
                break;
            case 1:
                il.Emit(OpCodes.Ldarg_1);
                break;
            case 2:
                il.Emit(OpCodes.Ldarg_2);
                break;
            case 3:
                il.Emit(OpCodes.Ldarg_3);
                break;
            default:
                if (index <= 255)
                    il.Emit(OpCodes.Ldarg_S, (byte)index);
                else
                    il.Emit(OpCodes.Ldarg, index);
                break;
        }
    }

    // 存储到局部变量
    public static void EmitStloc(ILCursor il, VariableDefinition local)
    {
        int index = local.Index;
        switch (index)
        {
            case 0:
                il.Emit(OpCodes.Stloc_0);
                break;
            case 1:
                il.Emit(OpCodes.Stloc_1);
                break;
            case 2:
                il.Emit(OpCodes.Stloc_2);
                break;
            case 3:
                il.Emit(OpCodes.Stloc_3);
                break;
            default:
                if (index <= 255)
                    il.Emit(OpCodes.Stloc_S, local);
                else
                    il.Emit(OpCodes.Stloc, local);
                break;
        }
    }

    // 加载局部变量地址
    public static void EmitLdloca(ILCursor il, VariableDefinition local)
    {
        int index = local.Index;
        if (index <= 255)
            il.Emit(OpCodes.Ldloca_S, local);
        else
            il.Emit(OpCodes.Ldloca, local);
    }

    // 加载局部变量
    public static void EmitLdloc(ILCursor il, VariableDefinition local)
    {
        int index = local.Index;
        switch (index)
        {
            case 0:
                il.Emit(OpCodes.Ldloc_0);
                break;
            case 1:
                il.Emit(OpCodes.Ldloc_1);
                break;
            case 2:
                il.Emit(OpCodes.Ldloc_2);
                break;
            case 3:
                il.Emit(OpCodes.Ldloc_3);
                break;
            default:
                if (index <= 255)
                    il.Emit(OpCodes.Ldloc_S, local);
                else
                    il.Emit(OpCodes.Ldloc, local);
                break;
        }
    }

    // 存储到参数
    public static void EmitStarg(ILCursor il, int index)
    {
        switch (index)
        {
            default:
                if (index <= 255)
                    il.Emit(OpCodes.Starg_S, (byte)index);
                else
                    il.Emit(OpCodes.Starg, index);
                break;
        }
    }
}
