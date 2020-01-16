using System;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IPropertyEditor
    {
        IPropertyEditor AddType(IXCType type);
        IPropertyEditor SetDatabaseColumnName(string name);
        IPropertyEditor SetGuid(Guid guid);
        IPropertyEditor SetName(string name);
    }
}