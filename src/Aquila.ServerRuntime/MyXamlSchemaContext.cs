using System;
using System.Reflection;
using Portable.Xaml;

namespace Aquila.ServerRuntime
{
    public class MyXamlSchemaContext : XamlSchemaContext
    {
        protected override Assembly OnAssemblyResolve(string assemblyName)
        {
            if (assemblyName.Contains("LibraryServer"))
            {
                var lookup = new AssemblyName(assemblyName);
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Matches(lookup))
                    {
                        Console.WriteLine("Founded!!!");
                        return assembly;
                    }
                }
            }

            return base.OnAssemblyResolve(assemblyName);
        }

        public override XamlType GetXamlType(Type type)
        {
            Console.WriteLine("THE TYPE IS :" + type);
            return new MyXamlType(type, this);
        }
    }
}