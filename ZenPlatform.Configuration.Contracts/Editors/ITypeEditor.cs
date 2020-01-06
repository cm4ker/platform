using System;


namespace ZenPlatform.Configuration.Contracts
{
    public interface ITypeEditor
    {
        IXCLinkType Link { get; }
        IXCObjectType Type { get; }

        ICommandEditor CreateCommand();
        IModuleEditor CreateModule();
        IPropertyEditor CreateProperty();
        ITypeEditor SetDescription(string description);
        ITypeEditor SetId(Guid id);
        ITypeEditor SetLinkId(Guid id);
        ITypeEditor SetName(string name);
        ITypeEditor SetRealTableName(string tableName);
    }
}