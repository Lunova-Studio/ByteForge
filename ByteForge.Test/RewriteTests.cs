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
        Assert.AreEqual(3, add); // ԭʼ�ӷ��߼�
        Assert.AreEqual(6, mul);

        byteForge.LoadMixin<RewriteMixin>();
        add = RewriteTestClass.Add(1, 2);
        mul = RewriteTestClass.Mul(2, 3);
        Assert.AreEqual(4, add); // �޸ĺ�ļӷ��߼�
        Assert.AreEqual(12, mul); // �޸ĺ�ĳ˷��߼�
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
        return a + b + 1; // �޸ļӷ��߼�
    }
    [At(nameof(RewriteTestClass.Mul), typeof(int), typeof(int), typeof(int))]
    [Rewrite]
    public static int RewriteMul(int a, int b)
    {
        return a * b * 2; // �޸ĳ˷��߼�
    }
}