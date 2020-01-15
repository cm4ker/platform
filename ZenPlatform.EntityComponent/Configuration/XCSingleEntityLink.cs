using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Описывает ссылку
    /// </summary>
    public class XCSingleEntityLink : XCLinkTypeBase
    {
        private readonly XCSingleEntityMetadata _metadata;

        private IXProperty _linkXProperty;

        public override string Name => $"{ParentType.Name}Link";

        public override Guid Guid => _metadata.LinkId;

        public override bool HasProperties => true;

        internal XCSingleEntityLink(IXCObjectType parentType, XCSingleEntityMetadata metadata)
        {
            _metadata = metadata;
            ParentType = parentType;

            _linkXProperty = StandardEntityPropertyHelper.CreateLinkProperty(this);
        }


        public override IEnumerable<IXProperty> GetProperties()
        {
            yield return _linkXProperty;
            
            foreach (var prop in _metadata.Properties)
            {
                yield return prop;
            }
        }
    }
}