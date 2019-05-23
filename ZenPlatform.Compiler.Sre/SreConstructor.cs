using System.Reflection;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    class SreConstructor : SreMethodBase, IConstructor
    {
        public ConstructorInfo Constuctor { get; }

        public SreConstructor(SreTypeSystem system, ConstructorInfo ctor) : base(system, ctor)
        {
            Constuctor = ctor;
        }

        public bool Equals(IConstructor other)
            => ((SreConstructor) other)?.Constuctor.Equals(Constuctor) == true;
    }
}