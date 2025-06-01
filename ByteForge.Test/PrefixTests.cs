using ByteForge.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByteForge.Test;

public class PrefixTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void PrefixTest()
    {
        ByteForge byteForge = new ByteForge();
        int add = PrefixTestClass.Add(1, 2);
        int mul = PrefixTestClass.Mul(2, 3);
        Assert.AreEqual(3, add); // 原始加法逻辑
        Assert.AreEqual(6, mul);

        byteForge.LoadMixin<PrefixMixin>();
        add = PrefixTestClass.Add(1, 2);
        mul = PrefixTestClass.Mul(2, 3);
        Assert.AreEqual(5, add); // 修改后的加法逻辑
        Assert.AreEqual(20, mul); // 修改后的乘法逻辑
        Assert.Pass();
    }
}

public class PrefixTestClass
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

[Mixin(typeof(PrefixTestClass))]
public class PrefixMixin
{
    [At(nameof(PrefixTestClass.Add), typeof(int), typeof(int), typeof(int))]
    [Prefix]
    public static void PrefixAdd(ref int a, ref int b)
    {
        a++;
        b++; // 修改加法逻辑，使得每个参数都加1
    }
    [At(nameof(PrefixTestClass.Mul), typeof(int), typeof(int), typeof(int))]
    [Prefix]
    public static void PrefixMul(ref int a, ref int b)
    {
        a += 2;
        b += 2; // 修改乘法逻辑，使得每个参数都加2
    }
}
