using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    public class SreTypeSystem : ITypeSystem
    {
        private List<IAssembly> _assemblies = new List<IAssembly>();
        public IReadOnlyList<IAssembly> Assemblies => _assemblies;

        private Dictionary<Type, SreType> _typeDic = new Dictionary<Type, SreType>();

        public SreTypeSystem()
        {
            // Ensure that System.ComponentModel is available
            var rasm = typeof(ISupportInitialize).Assembly;
            rasm = typeof(ITypeDescriptorContext).Assembly;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                try
                {
                    ResolveAssembly(asm);
                }
                catch
                {
                    //
                }
        }

        public IAssembly FindAssembly(string name)
        {
            return Assemblies.FirstOrDefault(a => a.Name.ToLowerInvariant() == name.ToLowerInvariant());
        }

        SreAssembly ResolveAssembly(Assembly asm)
        {
            if (asm.IsDynamic)
                return null;
            foreach (var a in Assemblies)
                if (((SreAssembly) a).Assembly == asm)
                    return (SreAssembly) a;
            var n = new SreAssembly(this, asm);
            _assemblies.Add(n);
            n.Init();
            return n;
        }

        internal SreType ResolveType(Type t)
        {
            if (_typeDic.TryGetValue(t, out var rv))
                return rv;
            _typeDic[t] = rv = new SreType(this, ResolveAssembly(t.Assembly), t);
            return rv;
        }

        public IType FindType(string name, string asm)
        {
            if (asm != null)
                name += ", " + asm;
            var found = Type.GetType(name);
            if (found == null)
                return null;
            return ResolveType(found);
        }

        public IType FindType(string name)
        {
            foreach (var asm in Assemblies)
            {
                var t = asm.FindType(name);
                if (t != null)
                    return t;
            }

            return null;
        }

        public IEmitter CreateCodeGen(MethodBuilder mb)
        {
            return new SreEmitter(this, new SreMethodEmitterProvider(mb));
        }

        public Type GetType(IType t) => ((SreType) t).Type;
        public IType GetType(Type t) => ResolveType(t);

        public ITypeBuilder CreateTypeBuilder(TypeBuilder builder) => new SreTypeBuilder(this, builder);


        public IAssembly GetAssembly(Assembly asm) => ResolveAssembly(asm);
    }
}