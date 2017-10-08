using System;
using System.Reflection;

namespace AssemblyGenerator.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //var asm = Assembly.GetEntryAssembly();
            //var asm = Assembly.LoadFile(@"C:\Users\Nikolay\Documents\Visual Studio 2017\AssemblyGenerator.Example\bin\Debug\netcoreapp2.0\AssemblyGenerator.dll");
            var asm = Assembly.LoadFile(@"C:\Users\Nikolay\Documents\Visual Studio 2017\Projects\AssemblyGenerator\src\AssemblyGenerator.Example\AssemblyGenerator.Example.dll");
            using (var generator = new AssemblyGenerator(asm))
            {
                generator.GenerateAssembly("output.dll");
            }

            Console.ReadKey();
        }
    }
}
