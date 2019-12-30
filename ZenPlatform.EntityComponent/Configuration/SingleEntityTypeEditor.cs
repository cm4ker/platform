using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Shared.ParenChildCollection;

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

            _metadata.EntityId = Guid.NewGuid();
            _metadata.LinkId = Guid.NewGuid();

            _type = new XCSingleEntity(_metadata);

            _link = new XCSingleEntityLink(_type, _metadata);


            ((IChildItem<IXCComponent>) _type).Parent = component;
            ((IChildItem<IXCComponent>) _link).Parent = component;


            _component = component;

            _component.RegisterType(_type);
            _component.Parent.RegisterType(_type);

            _component.RegisterType(_link);
            _component.Parent.RegisterType(_link);
        }

        public XCSingleEntity Type => _type;

        public XCSingleEntityLink Link => _link;

        public SingleEntityTypeEditor SetName(string name)
        {
            _metadata.Name = name;
            return this;
        }

        public SingleEntityTypeEditor SetDescription(string description)
        {
            return this;
        }

        public SingleEntityTypeEditor SetId(Guid id)
        {
            _metadata.EntityId = id;
            return this;
        }

        public SingleEntityTypeEditor SetLinkId(Guid id)
        {
            _metadata.LinkId = id;
            return this;
        }

        public SingleEntityTypeEditor SetRealTableName(string tableName)
        {
            _type.RelTableName = tableName;
            return this;
        }

        public SingleEntityPropertyEditor CreateProperty()
        {
            var newProperty = new XCSingleEntityProperty();
            newProperty.Guid = Guid.NewGuid();
            _metadata.Properties.Add(newProperty);

            return new SingleEntityPropertyEditor(newProperty);
        }

        public SingleEntityModuleEditor CreateModule()
        {
            var module = new XCSingleEntityModule();

            _metadata.Modules.Add(module);
            return new SingleEntityModuleEditor(module);
        }

        public SingleEntityCommandEditor CreateCommand()
        {
            var command = new XCCommand(true);
            _metadata.Command.Add(command);
            return new SingleEntityCommandEditor(command);
        }
    }
}