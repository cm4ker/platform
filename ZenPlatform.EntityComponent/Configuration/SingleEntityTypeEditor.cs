using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class SingleEntityTypeEditor
    {

        private XCSingleEntityMetadata _metadata;
        private XCSingleEntity _type;
        private XCSingleEntityLink _link;
        private IXCComponent _component;
        public SingleEntityTypeEditor(IXCComponent component)
        {
            _metadata = new XCSingleEntityMetadata();
            _type = new XCSingleEntity(_metadata);

            _link = new XCSingleEntityLink(_type, _metadata);

            _component = component;

            _component.RegisterType(_type);
            _component.Parent.RegisterType(_type);

            _component.RegisterType(_link);
            _component.Parent.RegisterType(_link);
        }

        public XCSingleEntity Type => _type;

        public XCSingleEntityLink Link => _link;

    }
}
