using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Editors;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class SingleEntityTypeEditor : ITypeEditor
    {
        private MDSingleEntity _metadata;
        private XCSingleEntity _type;
        private XCSingleEntityLink _link;
        private IXCComponent _component;

        public SingleEntityTypeEditor(IXCComponent component)
        {
            _metadata = new MDSingleEntity();

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

            _type.Initialize();
        }

        public IXCObjectType Type => _type;

        public IXCLinkType Link => _link;

        public ITypeEditor SetName(string name)
        {
            _metadata.Name = name;
            return this;
        }

        public ITypeEditor SetDescription(string description)
        {
            return this;
        }

        public ITypeEditor SetId(Guid id)
        {
            _metadata.EntityId = id;
            return this;
        }

        public ITypeEditor SetLinkId(Guid id)
        {
            _metadata.LinkId = id;
            return this;
        }

        public ITypeEditor SetRealTableName(string tableName)
        {
            _type.RelTableName = tableName;
            return this;
        }

        public ITableEditor CreateTable()
        {
            var newTable = new MDSingleEntityTable();
            newTable.Guid = Guid.NewGuid();
            _metadata.Tables.Add(newTable);

            return new SingleEntityTableEditor(newTable);
        }

        public IPropertyEditor CreateProperty()
        {
            var newProperty = new XCSingleEntityProperty();
            newProperty.Guid = Guid.NewGuid();
            _metadata.Properties.Add(newProperty);

            return new SingleEntityPropertyEditor(newProperty);
        }

        public IModuleEditor CreateModule()
        {
            var module = new XCSingleEntityModule();

            _metadata.Modules.Add(module);
            return new SingleEntityModuleEditor(module);
        }

        public ICommandEditor CreateCommand()
        {
            var command = new XCCommand(true);
            _metadata.Command.Add(command);
            return new SingleEntityCommandEditor(command);
        }
    }
}