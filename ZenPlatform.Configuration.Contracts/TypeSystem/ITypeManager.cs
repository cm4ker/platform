using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface ITypeManager
    {
        IType Int { get; }
        IType DateTime { get; }
        IType Binary { get; }
        IType String { get; }
        IType Boolean { get; }
        IType Guid { get; }
        IType Numeric { get; }
        IReadOnlyList<IType> Types { get; }
        IReadOnlyList<IProperty> Properties { get; }
        IReadOnlyList<ITable> Tables { get; }
        IReadOnlyList<IComponent> Components { get; }


        void Register(IType type);
        void Register(IProperty p);
        void Register(IPropertyType type);
        IType Type();
        ITypeSpec Type(IType type);
        IProperty Property();
        IPropertyType PropertyType();
        ITable Table();
        void Verify();
        void Register(IComponent component);
        IComponent Component();
        void LoadSettings(IEnumerable<IObjectSetting> settings);
    }
}