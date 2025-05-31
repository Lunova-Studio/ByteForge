using ByteForge.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteForge.Test
{
    public class Class1
    {
        private int field = 2;
        public static void test()
        {
            Console.WriteLine("Hello, World!");
        }

        public static int add(int i, int j)
        {
            return i + j;
        }

        public int getField()
        {
            return field;
        }

        public int getField2(int i)
        {
            return field + i;
        }

        public static int ret(int i)
        {
            return i + 1;
        }

        public static void WriteLine(string s) { Console.WriteLine(s); }

        public static int add0(int i, int j)
        {
            return i + j;
        }

        public static int add1(int i, int j)
        {
            int result = add0(i, j);
            return add0(12, result);
        }
    }

    [Mixin(typeof(Class1))]
    public abstract class Class2
    {
        [Shadow]
        private int field;

        [At(nameof(test), null)]
        [Rewrite]
        public static void test()
        {
            Console.WriteLine("Hello, World! Rewrited!");
        }

        [At(nameof(add), typeof(int), typeof(int), typeof(int))]
        [Rewrite]
        public static int add(int i, int j)
        {
            return 250;
        }

        [At(nameof(getField), typeof(int))]
        [Rewrite]
        public int getField()
        {
            return field + 10;
        }

        [At(nameof(getField2), typeof(int), typeof(int))]
        [Postfix]
        public void getField2(ref int i, ref int result)
        {
            result = field + 10000000;
        }

        [At(nameof(ret), typeof(int), typeof(int))]
        [Prefix]
        public static void ret(ref int i)
        {
            i = 10000;
        }

        [At(nameof(Class1.ret), typeof(int), typeof(int))]
        [Postfix]
        public static void ret1(ref int i, ref int result)
        {
            result = i - 60000;
        }

        [At(nameof(WriteLine), null, typeof(string))]
        [Prefix]
        public static void WriteLine(ref string s)
        {
            s += " prefixed!";
        }

        [At(nameof(WriteLine), null, typeof(string))]
        [Prefix]
        public static void WriteLine2(ref string s)
        {
            s += " prefixed2!";
        }

        [At("add1", typeof(int), typeof(int), typeof(int))]
        [ModifyArgument(0, 1, typeof(Class1), "add0", typeof(int), typeof(int), typeof(int))]
        public static void add1Modify(ref int i)
        {
            i = 234;
        }
    }

    [Mixin(typeof(Console))]
    public class Class3
    {
        [At("WriteLine", null, typeof(string))]
        [Prefix]
        public static void Pre(ref string s)
        {
            s += " Prefixed!";
        }

        [At("WriteLine", null, typeof(string))]
        [Postfix]
        public static void Post(ref string s)
        {
            s += " Postfixed!";
        }
    }
}
