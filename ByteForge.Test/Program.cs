using Mono.Cecil;

namespace ByteForge.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ByteForge byteForge = new ByteForge();
            //Console.WriteLine("Hello World");
            //byteForge.LoadMixin<Class3>();
            //Console.WriteLine("Hello World");
            Class1.test();
            Console.WriteLine(Class1.add(1, 2));
            Console.WriteLine(new Class1().getField());
            Console.WriteLine(new Class1().getField2(22));
            Console.WriteLine(Class1.ret(2));
            Class1.WriteLine("hello");
            byteForge.LoadMixin<Class2>();
            Class1.test();
            Console.WriteLine(Class1.add(1, 2));
            Console.WriteLine(new Class1().getField());
            Console.WriteLine(new Class1().getField2(22));
            Console.WriteLine(Class1.ret(2));
            Class1.WriteLine("hello");
        }
    }
}
