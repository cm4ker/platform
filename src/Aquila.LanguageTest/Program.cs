using System;
using System.Collections.Generic;
using CompileNamespace;

namespace Aquila.LanguageTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Action> a = new List<Action>();

            for (int i = 0; i < 10; i++)
            { 
                var c = i;
              a.Add(()=>
              {
                  Console.WriteLine(c);
              });
            }

            foreach (var action in a)
            {
                action();
            }
        }
    }
}