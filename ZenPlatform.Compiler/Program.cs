using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Antlr4.Runtime;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Sre;
using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Main2(args);
        }

        static void Main2(string[] args)
        {
            string test;
            var text = new StringReader(@"
module Test
{
    double Inc(int a)
    {
        #if TEST
        a++;
        #else
        a = a + 2;
        #endif
        return 0.0;
    }

    int Add(int a, int b)
    {
        int c = 1;
        try
        {
            c = c + 2;
            //return a + b;
        }
        catch
        {
            c++;//return 0;
        }
        
        int i = c + a;
        
        return i;
    }

    int Fibonachi(int n)
    {
        if(n == 0) return 0;
        if(n == 1) return 1;

        return Fibonachi(n-2) + Fibonachi(n-1);
    }

    double Average(int[] arr)
    {
        double result = 0.0;

        for(int i = 0; i < arr.Length; i++)
        {
            result = result + (double)arr[i];
        }  

        result = result / (double)(arr.Length);
        
        return result;
    }

    int Sort(int[] arr)
    {
        int temp = 0;

        for (int write = 0; write < arr.Length; write++) 
        {
            for (int sort = 0; sort < arr.Length - 1; sort++) 
            {
                if (arr[sort] > arr[sort + 1]) 
                {
                    temp = arr[sort + 1];
                    arr[sort + 1] = arr[sort];
                    arr[sort] = temp;
                }
            }
        }
        return 2 + 2 * 2;
    }

    void VoidFunction()
    {
        int a = 1;
    }
}
");

            CompilationBackend cb = new CompilationBackend();
            var b = cb.Compile(text);

            if (File.Exists("Debug.dll"))
                File.Delete("Debug.dll");

            b.Write("Debug.dll");
        }
    }
}