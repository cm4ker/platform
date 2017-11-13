using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Data;
using ZenPlatform.ConfigurationDataComponent;
using ZenPlatform.Core.Entity;
using ZenPlatform.CSharpCodeBuilder.Syntax;
using ZenPlatform.DataComponent;


namespace ZenPlatform.DocumentComponent
{
    public class DocumnetComponent : DataComponentBase
    {
        public DocumnetComponent(PComponent component) : base(component)
        {
            Generator = new DocumentEntityGenerator();
            EntityBase = typeof(DocumentEntity);
            Manager = new DocumentManager();
            Initialization();
        }

        private void Initialization()
        {
            Component.RegisterCodeRule(Generator.GetInForeignPropertySetActionRule());
            Component.RegisterCodeRule(Generator.GetInForeignPropertyGetActionRule());
            Component.RegisterCodeRule(Generator.GetEntityClassPostfixRule());
            Component.RegisterCodeRule(Generator.GetEntityClassPrefixRule());

            Component.RegisterCodeRule(Generator.GetNamespaceRule());
        }

        public override Type EntityBase { get; }
        public override EntityManagerBase Manager { get; }
        public override EntityGeneratorBase Generator { get; }



    }
}
