using System;
using System.CodeDom.Compiler;
using System.Data;
using System.Text;
using CodeGeneration.Syntax;
using Microsoft.CSharp;
using SqlPlusDbSync.Platform.EntityObject;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Platform
{
    public class EntityGenerator
    {
        public EntityGenerator()
        {
        }

        public void Generate(Core core)
        {
            var doc = new DocumentSyntax();
            var ns = new NamespaceSyntax("EntityLibrary");

            doc.AddNamespace(ns);

            doc.AddUsing(new UsingSyntax("System"));
            doc.AddUsing(new UsingSyntax("System.Collections.Generic"));
            doc.AddUsing(new UsingSyntax("System.Reflection"));

            var assemblyVersion = new AttributeSyntax("AssemblyVersion", "assembly");
            assemblyVersion.AddConstrunctorParam(new StringValueSyntax("1.0.0.0"));

            doc.AddAttribute(assemblyVersion);

            //doc.AddUsing(new UsingSyntax("SqlPlusDbSync.Platform"));
            doc.AddUsing(new UsingSyntax(typeof(DTOObject).Namespace));
            foreach (var sobject in core.SupportedObjects)
            {
                var cls = new ClassSyntax(sobject.Name, new TypeSyntax(typeof(DTOObject)));
                //var attr = new AttributeSyntax("Table");
                //attr.AddConstrunctorParam(new StringValueSyntax(sobject.GetTableObject().Name));

                foreach (var property in sobject.Fields)
                {
                    var prop = new PropertySyntax(property.Name, new TypeSyntax(PlatformHelper.GetClrType(property.Schema.Type, property.Schema.IsNullable)));
                    cls.AddProperty(prop);
                }
                if (!(sobject is TableType))
                    foreach (var table in sobject.Relations)
                    {
                        var prop = new PropertySyntax(table.Type.Name, new TypeSyntax($"List<{table.Type.Name}>"));
                        cls.AddProperty(prop);
                    }

                //cls.AddAttribute(attr);
                ns.AddClass(cls);
            }

            DynamicAssemblyCompiller asmCompiller = new DynamicAssemblyCompiller();
            asmCompiller.Compile(doc.ToString());
        }

        public void GenerateLinkedEntity(Core core)
        {
            var doc = new DocumentSyntax();
            var ns = new NamespaceSyntax("EntityLibrary");

            doc.AddNamespace(ns);

            doc.AddUsing(new UsingSyntax("System"));
            doc.AddUsing(new UsingSyntax("System.Collections.Generic"));
            doc.AddUsing(new UsingSyntax("System.Reflection"));

            var assemblyVersion = new AttributeSyntax("AssemblyVersion", "assembly");
            assemblyVersion.AddConstrunctorParam(new StringValueSyntax("1.0.0.0"));

            doc.AddAttribute(assemblyVersion);

            //doc.AddUsing(new UsingSyntax("SqlPlusDbSync.Platform"));
            doc.AddUsing(new UsingSyntax(typeof(DTOObject).Namespace));
            foreach (var sobject in core.SupportedObjects)
            {
                var cls = new ClassSyntax(sobject.Name, new TypeSyntax(typeof(DTOObject)));
                //var attr = new AttributeSyntax("Table");
                //attr.AddConstrunctorParam(new StringValueSyntax(sobject.GetTableObject().Name));

                foreach (var property in sobject.Fields)
                {
                    var prop = new PropertySyntax(property.Name, new TypeSyntax(PlatformHelper.GetClrType(property.Schema.Type, property.Schema.IsNullable)));
                    cls.AddProperty(prop);
                }
                if (!(sobject is TableType))
                    foreach (var table in sobject.Relations)
                    {
                        var prop = new PropertySyntax(table.Type.Name, new TypeSyntax($"List<{table.Type.Name}>"));
                        cls.AddProperty(prop);
                    }

                //cls.AddAttribute(attr);
                ns.AddClass(cls);
            }

            DynamicAssemblyCompiller asmCompiller = new DynamicAssemblyCompiller();
            asmCompiller.Compile(doc.ToString());

        }

        public class DynamicAssemblyCompiller
        {
            public void Compile(string code)
            {
                string dynamicCode = code;
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters();

                // Reference to System.Drawing library

                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Core.dll");
                parameters.ReferencedAssemblies.Add("System.Data.dll");
                parameters.ReferencedAssemblies.Add("System.Data.Linq.dll");
                parameters.ReferencedAssemblies.Add("System.Xml.dll");
                parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
                parameters.ReferencedAssemblies.Add("System.ComponentModel.DataAnnotations.dll");
                parameters.ReferencedAssemblies.Add("SqlPlusDbSync.Platform.dll");

                // True - memory generation, false - external file generation
                parameters.GenerateInMemory = false;
                // True - exe file generation, false - dll file generation
                parameters.GenerateExecutable = false;
                parameters.OutputAssembly = "EntityAssembly.dll";

                CompilerResults results = provider.CompileAssemblyFromSource(parameters, dynamicCode);


                if (results.Errors.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (CompilerError error in results.Errors)
                    {
                        sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                    }

                    Logger.LogWarning(sb.ToString());
                }
            }
        }
    }
}
