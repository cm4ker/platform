using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class SingleEntityPropertyEditor
    {

        private XCSingleEntityProperty _property;
        public SingleEntityPropertyEditor(XCSingleEntityProperty property)
        {
            _property = property;
        }

        public SingleEntityPropertyEditor SetName(string name)
        {
            _property.Name = name;
            return this;
        }

        public SingleEntityPropertyEditor AddType(IXCType type)
        {
            _property.Types.Add(type);
            return this;
        }

        public SingleEntityPropertyEditor SetGuid(Guid guid)
        {
            _property.Guid = guid;
            return this;
        }

        public SingleEntityPropertyEditor SetDatabaseColumnName(string name)
        {
            _property.DatabaseColumnName = name;
            return this;
        }


    }
}
