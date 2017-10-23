//using System;
//using System.CodeDom.Compiler;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using Microsoft.CSharp;
//using SqlPlusDbSync.Platform.EntityObject;

//namespace SqlPlusDbSync.Platform
//{
//    class DynamicExecutor
//    {

//        public void ExecuteCode(string code, DTOObject dtoObject, InvokeEventArgs args)
//        {
//            string dynamicCode = $@"
//    using System;
//    using System.Dynamic;
//    using System.Linq;
//    using System.Text;
//    using {dtoObject.GetType().Namespace};
//    using {typeof(InvokeEventArgs).Namespace};
//    using {typeof(DTOObject).Namespace};

//    namespace DynamicNamespace
//    {{
//        public class DynamicCode
//        {{
//            public static void Main(){{}}

//            public static void Main(Entity sender, InvokeEventArgs args)
//            {{
//                {code}
//            }}
//        }}
//    }}
//";
//            using (CodeDomProvider provider = CodeDomProvider.CreateProvider("C#"))
//            {
//                CompilerParameters parameters = new CompilerParameters();


//                // Reference to System.Drawing library

//                parameters.ReferencedAssemblies.Add("System.dll");
//                parameters.ReferencedAssemblies.Add("System.Core.dll");
//                parameters.ReferencedAssemblies.Add("System.Data.dll");
//                parameters.ReferencedAssemblies.Add("System.Data.Linq.dll");
//                parameters.ReferencedAssemblies.Add("System.Xml.dll");
//                parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
//                parameters.ReferencedAssemblies.Add("SqlPlusDbSync.Platform.dll");
//                parameters.ReferencedAssemblies.Add("EntityAssembly.dll");

//                // True - memory generation, false - external file generation
//                parameters.GenerateInMemory = true;
//                // True - exe file generation, false - dll file generation
//                parameters.GenerateExecutable = true;

//                CompilerResults results = provider.CompileAssemblyFromSource(parameters, dynamicCode);

//                if (results.Errors.HasErrors)
//                {
//                    StringBuilder sb = new StringBuilder();

//                    foreach (CompilerError error in results.Errors)
//                    {
//                        sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
//                    }

//                    throw new InvalidOperationException(sb.ToString());
//                }

//                Assembly assembly = results.CompiledAssembly;
//                Type program = assembly.GetType("DynamicNamespace.DynamicCode");
//                MethodInfo main = program.GetMethod("Main", new Type[] { typeof(DTOObject), typeof(InvokeEventArgs) });
//                try
//                {
//                    main.Invoke(this, new object[] { dtoObject, args });
//                }
//                catch (Exception ex)
//                {
//                    throw new Exception($"Error in rules. Message: {ex.Message} StackTrace: {ex.StackTrace}");
//                }
//                finally
//                {
//                    provider.Dispose();
//                }
//            }
//        }
//    }
//}
