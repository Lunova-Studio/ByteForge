using ByteForge.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByteForge.Test;

public class PostfixTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void PostfixTest()
    {
        ByteForge byteForge = new ByteForge();
        int add = PostfixTestClass.Add(1, 2);
        int mul = PostfixTestClass.Mul(2, 3);
        Assert.AreEqual(3, add); // 原始加法逻辑
        Assert.AreEqual(6, mul);

        byteForge.LoadMixin<PostfixMixin>();
        add = PostfixTestClass.Add(1, 2);
        mul = PostfixTestClass.Mul(2, 3);
        Assert.AreEqual(4, add); // 修改后的加法逻辑
        Assert.AreEqual(7, mul); // 修改后的乘法逻辑
        Assert.Pass();
    }
}

public class PostfixTestClass
{
    public static int Add(int a, int b)
    {
        return a + b;
    }

    public static int Mul(int a, int b)
    {
        return a * b;
    }
}

[Mixin(typeof(PostfixTestClass))]
public class PostfixMixin
{
    [At(nameof(PostfixTestClass.Add), typeof(int), typeof(int), typeof(int))]
    [Postfix]
    public static void PostfixAdd(ref int a, ref int b, ref int result)
    {
        result++; // 修改加法逻辑，使得结果加1
    }
    [At(nameof(PostfixTestClass.Mul), typeof(int), typeof(int), typeof(int))]
    [Postfix]
    public static void PostfixMul(ref int a, ref int b, ref int result)
    {
        result++; // 修改乘法逻辑，使得结果加1
    }
}
