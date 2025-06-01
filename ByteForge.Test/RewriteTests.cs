using ByteForge.Attributes;

namespace ByteForge.Test;

public class RewriteTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void RewriteTest()
    {
        ByteForge byteForge = new ByteForge();
        int add = RewriteTestClass.Add(1, 2);
        int mul = RewriteTestClass.Mul(2, 3);
        Assert.AreEqual(3, add); // 原始加法逻辑
        Assert.AreEqual(6, mul);

        byteForge.LoadMixin<RewriteMixin>();
        add = RewriteTestClass.Add(1, 2);
        mul = RewriteTestClass.Mul(2, 3);
        Assert.AreEqual(4, add); // 修改后的加法逻辑
        Assert.AreEqual(12, mul); // 修改后的乘法逻辑
        Assert.Pass();
    }
}

public class RewriteTestClass
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

[Mixin(typeof(RewriteTestClass))]
public class RewriteMixin
{
    [At(nameof(RewriteTestClass.Add), typeof(int), typeof(int), typeof(int))]
    [Rewrite]
    public static int RewriteAdd(int a, int b)
    {
        return a + b + 1; // 修改加法逻辑
    }
    [At(nameof(RewriteTestClass.Mul), typeof(int), typeof(int), typeof(int))]
    [Rewrite]
    public static int RewriteMul(int a, int b)
    {
        return a * b * 2; // 修改乘法逻辑
    }
}