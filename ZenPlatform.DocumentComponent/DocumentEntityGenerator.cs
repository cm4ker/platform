using System;
using System.Linq;
using ZenPlatform.Configuration.Data;
using ZenPlatform.ConfigurationDataComponent;
using ZenPlatform.CSharpCodeBuilder.Syntax;
using ZenPlatform.DataComponent;

namespace ZenPlatform.DocumentComponent
{
    public class DocumentEntityGenerator : EntityGeneratorBase
    {
        //public override void GenerateEntityClass(DocumentSyntax document, PObjectType conf)
        //{
        //    if (conf.IsAbstractType) return;

        //    ClassSyntax cs = new ClassSyntax(conf.Name);

        //    foreach (var prop in conf.Propertyes)
        //    {
        //        if (prop.Types.Count == 1)
        //        {
        //            var propType = prop.Types.First();

        //            if (propType is PObjectType)
        //                cs.AddProperty(new PropertySyntax(prop.Name, new TypeSyntax(propType.Name)));

        //            if (propType is PPrimetiveType primitiveType)
        //                cs.AddProperty(new PropertySyntax(prop.Name, new TypeSyntax(primitiveType.CLRType)));
        //        }
        //        else
        //        {
        //            bool alreadyHaveObjectTypeField = false;
        //            foreach (var type in prop.Types)
        //            {
        //                if (type is PObjectType && !alreadyHaveObjectTypeField)
        //                {
        //                    cs.AddProperty(new PropertySyntax($"{prop.Name}_Ref", new TypeSyntax(typeof(Guid))));
        //                    cs.AddProperty(new PropertySyntax($"{prop.Name}_Type", new TypeSyntax(typeof(int))));
        //                    alreadyHaveObjectTypeField = true;
        //                }

        //                if (type is PPrimetiveType primitiveType)
        //                    cs.AddProperty(new PropertySyntax($"{prop.Name}_{primitiveType.Name}", new TypeSyntax(primitiveType.CLRType)));
        //            }
        //        }
        //    }
        //}
    }
}