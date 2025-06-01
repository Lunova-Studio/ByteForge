using ByteForge.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByteForge.Test;

public class ModifyArgumentTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ModifyArgumentTest()
    {
        ByteForge byteForge = new ByteForge();
        byteForge.LoadMixin<ModifyArgumentMixin>();
        Assert.AreEqual(2, ModifyArgumentTestClass.Foo(2, 3));
        Assert.Pass();
    }
}

public class ModifyArgumentTestClass
{
    public static int Foo(int a, int b)
    {
        int t = Bar(a, b);
        t *= b;
        t += a;
        t = Bar(t, 5);
        return t + 2;
    }

    public static int Bar(int a, int b)
    {
        return a * b;
    }
}

[Mixin(typeof(ModifyArgumentTestClass))]
public class ModifyArgumentMixin
{
    [At(nameof(ModifyArgumentTestClass.Foo), typeof(int), typeof(int), typeof(int))]
    [ModifyArgument(0, 1, typeof(ModifyArgumentTestClass), nameof(ModifyArgumentTestClass.Bar), typeof(int), typeof(int), typeof(int))]
    public static void Foo(ref int t)
    {
        t = 0;
    }
}
