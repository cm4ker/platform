using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts
{
    public interface ITypeBuilder : IType
    {
        void AddInterfaceImplementation(IType type);

        void DefineGenericParameters(IReadOnlyList<KeyValuePair<string, GenericParameterConstraint>> names);

        IReadOnlyList<IMethodBuilder> DefinedMethods { get; }

        IField DefineField(IType type, string name, bool isPublic, bool isStatic);

        IMethodBuilder DefineMethod(string name,
            bool isPublic, bool isStatic,
            bool isInterfaceImpl, IMethod overrideMethod = null);

        IPropertyBuilder DefineProperty(IType propertyType, string name);

        IConstructorBuilder DefineConstructor(bool isStatic, params IType[] args);

        ITypeBuilder DefineNastedType(IType baseType, string name, bool isPublic);

        IType EndBuild();
    }
}