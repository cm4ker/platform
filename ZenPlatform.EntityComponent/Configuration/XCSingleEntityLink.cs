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
        private readonly ImdSingleEntity _metadata;

        private IXCProperty _linkIxcProperty;

        public override string Name => $"{ParentType.Name}Link";

        public override Guid Guid => _metadata.LinkId;

        public override bool HasProperties => true;
        public override bool IsSealed => false;
        public override bool IsAbstract => false;
        public override bool HasModules => false;
        public override bool HasCommands => false;
        public override bool HasDatabaseUsed => false;

        internal XCSingleEntityLink(IXCObjectType parentType, ImdSingleEntity metadata)
        {
            _metadata = metadata;
            ParentType = parentType;

            _linkIxcProperty = StandardEntityPropertyHelper.CreateLinkProperty(this);
        }


        public override IEnumerable<IXCProperty> GetProperties()
        {
            yield return _linkIxcProperty;
            
            foreach (var prop in _metadata.Properties)
            {
                yield return prop;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is XCSingleEntityLink link &&
                   Guid.Equals(link.Guid);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Guid);
        }
    }
}