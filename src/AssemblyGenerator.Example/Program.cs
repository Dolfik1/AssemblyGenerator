using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace AssemblyGenerator.Example
{
    /*
    public class GenericClassTest<T>
    {
        private T _genericVar { get; set; }
        public GenericClassTest(T test)
        {
            _genericVar = test;
        }
    }
    */
    
    public class SampleFactorialFromEmission
    {
        // emit the assembly using op codes
        public static Assembly EmitAssembly(int theValue)
        {
            // create assembly name
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "FactorialAssembly";

            // create assembly with one module
            AssemblyBuilder newAssembly =
                AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder newModule = newAssembly.DefineDynamicModule("MFactorial");

            // define a public class named "CFactorial" in the assembly
            TypeBuilder myType = newModule.DefineType("Namespace.CFactorial", TypeAttributes.Public);

            // Mark the class as implementing IFactorial.
            // myType.AddInterfaceImplementation(typeof(IFactorial));

            // define myfactorial method by passing an array that defines
            // the types of the parameters, the type of the return type,
            // the name of the method, and the method attributes.

            Type[] paramTypes = new Type[0];
            Type returnType = typeof(Int32);
            MethodBuilder simpleMethod = myType.DefineMethod("myfactorial",
                MethodAttributes.Public | MethodAttributes.Virtual,
                returnType,
                paramTypes);

            // obtain an ILGenerator to emit the IL
            ILGenerator generator = simpleMethod.GetILGenerator();

            // Ldc_I4 pushes a supplied value of type int32
            // onto the evaluation stack as an int32.
            // push 1 onto the evaluation stack.
            // foreach i less than theValue,
            // push i onto the stack as a constant
            // multiply the two values at the top of the stack.
            // The result multiplication is pushed onto the evaluation
            // stack.
            generator.Emit(OpCodes.Ldc_I4, 1);

            for (Int32 i = 1; i <= theValue; ++i)
            {
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Mul);
            }

            // emit the return value on the top of the evaluation stack.
            // Ret returns from method, possibly returning a value.

            generator.Emit(OpCodes.Ret);

            // encapsulate information about the method and
            // provide access to the method metadata
            // MethodInfo factorialInfo = typeof(IFactorial).GetMethod("myfactorial");

            // specify the method implementation.
            // pass in the MethodBuilder that was returned
            // by calling DefineMethod and the methodInfo just created
            // myType.DefineMethodOverride(simpleMethod, factorialInfo);

            // create the type and return new on-the-fly assembly
            myType.CreateType();

            return newAssembly;
        }
    }


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
            // var asm = Assembly.GetEntryAssembly();
            // var asm = Assembly.LoadFile(@"D:\X-Files\Projects\AssemblyGenerator\src\AssemblyGenerator.SampleAssembly\bin\Debug\netstandard2.0\AssemblyGenerator.SampleAssembly.dll");
            //var asm = Assembly.LoadFile(@"C:\Users\Nikolay\Documents\Visual Studio 2017\Projects\AssemblyGenerator\src\AssemblyGenerator.Example\AssemblyGenerator.Example.dll");
            var asm = SampleFactorialFromEmission.EmitAssembly(10);
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

        void NonStaticMethod()
        {

        }

    }
}
