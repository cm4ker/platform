/*
 Необходимо создать сущности трёх типов типов:
 1) Дата сущности (DTO Objects)
 2) Сущности с предопределённой логикой (Entity)
 3) Кастомные сущности (Classes), которые олицитворяют собой класс обработки данных и не хранятся в базе данных
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.PlatformCodeGenerator
{
//    public class DTOGenerator
//    {
//        private readonly List<PObjectType> _pobjects;
//        private DynamicAssemblyCompiller _asmCompiller;
//        private const string Namespace = "DTOLibrary";
//
//        public DTOGenerator(List<PObjectType> pobjects)
//        {
//            _pobjects = pobjects;
//
//            _asmCompiller = new DynamicAssemblyCompiller();
//
//        }
//
//        public void Generate()
//        {
//            _asmCompiller.Compile(GetModuleText());
//        }
//
//        public string GetModuleText()
//        {
//            var doc = new DocumentSyntax();
//            var ns = new NamespaceSyntax(Namespace);
//
//            doc.AddNamespace(ns);
//
//            doc.AddUsing(new UsingSyntax("System"));
//            doc.AddUsing(new UsingSyntax("System.Collections.Generic"));
//            doc.AddUsing(new UsingSyntax("System.Reflection"));
//
//            var assemblyVersion = new AttributeSyntax("AssemblyVersion", "assembly");
//            assemblyVersion.AddConstrunctorParam(new StringValueSyntax("1.0.0.0"));
//
//            doc.AddAttribute(assemblyVersion);
//
//            doc.AddUsing(new UsingSyntax(typeof(DTO).Namespace));
//
//            foreach (var pObjectType in _pobjects)
//            {
//                if (string.IsNullOrEmpty(pObjectType.Name)) throw new Exception("Object must have name");
//
//                var cls = new ClassSyntax(pObjectType.Name);
//
//                foreach (var property in pObjectType.Propertyes)
//                {
//
//
//                    if (property.Types.Count == 0)
//                        throw new Exception("Property MUST have any type");
//
//                    if (property.Types.Count == 1)
//                    {
//                        PropertySyntax prop;
//                        var ptype = property.Types.First();
//                        if (ptype is PPrimetiveType)
//                        {
//                            var primitiveType = ptype as PPrimetiveType;
//                            prop = new PropertySyntax(property.Name, new TypeSyntax(primitiveType.CLRType));
//                        }
//                        else
//                        {
//                            var objectType = ptype as PObjectType;
//                            prop = new PropertySyntax(property.Name, new TypeSyntax(typeof(Guid)));
//                        }
//
//                        cls.AddProperty(prop);
//                    }
//                    else
//                    {
//                        PropertySyntax typeSplitter = null;
//                        foreach (var type in property.Types)
//                        {
//                            if (type is PPrimetiveType)
//                            {
//                                cls.AddProperty(new PropertySyntax($"{property.Name}_{type.Name}", new TypeSyntax((type as PPrimetiveType).CLRType)));
//                            }
//                            if (type is PObjectType)
//                            {
//                                cls.AddProperty(new PropertySyntax($"{property.Name}_RefType", new TypeSyntax(typeof(byte))));
//                                cls.AddProperty(new PropertySyntax($"{property.Name}_Ref", new TypeSyntax(typeof(Guid))));
//                            }
//                        }
//                    }
//                }
//                ns.AddClass(cls);
//            }
//
//            return doc.ToString();
//        }
//
//        public class DynamicAssemblyCompiller
//        {
//            public void Compile(string code)
//            {
//                string dynamicCode = code;
//                CSharpCodeProvider provider = new CSharpCodeProvider();
//                CompilerParameters parameters = new CompilerParameters();
//
//                // Reference to System.Drawing library
//
//                parameters.ReferencedAssemblies.Add("System.dll");
//                parameters.ReferencedAssemblies.Add("System.Core.dll");
//                parameters.ReferencedAssemblies.Add("System.Data.dll");
//                parameters.ReferencedAssemblies.Add("System.Data.Linq.dll");
//                parameters.ReferencedAssemblies.Add("System.Xml.dll");
//                parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
//                parameters.ReferencedAssemblies.Add("System.ComponentModel.DataAnnotations.dll");
//                parameters.ReferencedAssemblies.Add("SqlPlusDbSync.Platform.dll");
//
//                // True - memory generation, false - external file generation
//                parameters.GenerateInMemory = false;
//                // True - exe file generation, false - dll file generation
//                parameters.GenerateExecutable = false;
//                parameters.OutputAssembly = "EntityAssembly.dll";
//
//                CompilerResults results = provider.CompileAssemblyFromSource(parameters, dynamicCode);
//
//
//                if (results.Errors.HasErrors)
//                {
//                    StringBuilder sb = new StringBuilder();
//
//                    foreach (CompilerError error in results.Errors)
//                    {
//                        sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
//                    }
//
//                    //Logger.LogWarning(sb.ToString());
//                }
//            }
//        }
//    }
}
