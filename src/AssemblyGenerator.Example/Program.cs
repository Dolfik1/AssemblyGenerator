using System;
using System.Reflection;

namespace AssemblyGenerator.Example
{
    class Program
    {
        public string Test = "";
        public int TestInt = 0;

        public string TestProp { get; set; }
        private string TestPrivateProp { get; set; }
        private static string TestPrivateStaticProp { get; set; }
        public string TestReadonlyProp { get; }
        public string TestWriteonlyProp
        {
            set
            {
            }
        }

        static void Main(string[] args)
        {
            var asm = Assembly.GetEntryAssembly();
            //var asm = Assembly.LoadFile(@"C:\Users\Nikolay\Documents\Visual Studio 2017\AssemblyGenerator.Example\bin\Debug\netcoreapp2.0\AssemblyGenerator.dll");
            //var asm = Assembly.LoadFile(@"C:\Users\Nikolay\Documents\Visual Studio 2017\Projects\AssemblyGenerator\src\AssemblyGenerator.Example\AssemblyGenerator.Example.dll");
            using (var generator = new AssemblyGenerator(asm))
            {
                generator.GenerateAssembly("output.dll");
            }

            //Console.WriteLine("READY!");
            //Console.ReadKey();
        }

        static void MainTestInt(int x)
        {

        }

        static void MainTestIntArr(int[] x)
        {

        }

        static void MainTestIntArrMultiple(int[,,,] x)
        {

        }

        static int MainTestIntReturn(int x)
        {
            return x;
        }
    }
}
