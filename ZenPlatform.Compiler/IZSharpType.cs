using ZenPlatform.Compiler.Cecil.Backend;

namespace ZenPlatform.Compiler
{
    public interface ITypeBuilder
    {
        IPropertyBuilder CreateProperty(string name);
    }

    public interface IPropertyBuilder
    {
        Emitter GetMethodEmitter { get; }
        Emitter SetMethodEmitter { get; }

        string Name { get; }
    }
}