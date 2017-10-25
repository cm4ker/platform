using System;
using ZenPlatform.Configuration.Data;
using ZenPlatform.CSharpCodeBuilder.Syntax;

namespace ZenPlatform.ConfigurationDataComponent
{
    public abstract class EntityGeneratorBase
    {
        protected EntityGeneratorBase()
        {

        }


        public virtual void GenerateEntityClass(DocumentSyntax document, PObjectType conf)
        {
     //
            throw new NotImplementedException();
        }
    }
}