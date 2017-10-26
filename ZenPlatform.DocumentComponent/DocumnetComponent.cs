using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Data;
using ZenPlatform.ConfigurationDataComponent;
using ZenPlatform.CSharpCodeBuilder.Syntax;
using ZenPlatform.DataComponent;

namespace ZenPlatform.DocumentComponent
{
    public class DocumnetComponent : DataComponentBase
    {
        public DocumnetComponent()
        {
            Generator = new DocumentEntityGenerator();
            EntityBase = typeof(DocumentEntity);
            Manager = new DocumentManager(new Document2Sql(), null);
        }

        public override IEnumerable<PPrimetiveType> GetSupportedEntityPrimitiveTypes()
        {
            throw new NotImplementedException();
        }

        public override Type EntityBase { get; }
        public override EntityManagerBase Manager { get; }
        public override EntityGeneratorBase Generator { get; }
    }
}
