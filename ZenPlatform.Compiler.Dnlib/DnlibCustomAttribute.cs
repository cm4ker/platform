using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibCustomAttribute : ICustomAttribute
    {
        private readonly DnlibTypeSystem _ts;
        private readonly CustomAttribute _ca;


        public DnlibCustomAttribute(DnlibTypeSystem ts, CustomAttribute ca)
        {
            _ts = ts;
            Resolver = new DnlibContextResolver(ts, ca.Constructor.Module);
            TypeSystem = ts;
            _ca = ca;
        }

        protected DnlibContextResolver Resolver { get; }

        internal DnlibTypeSystem TypeSystem { get; }

        internal CustomAttribute CustomAttribute => _ca;

        public bool Equals(ICustomAttribute other)
        {
            if (other is DnlibCustomAttribute a)
                return _ca.Equals(a._ca);

            return false;
        }

        public List<object> Parameters => _ca.ConstructorArguments.Select(x => x.Value).ToList();

        public Dictionary<string, object> Properties => _ca.Properties.ToDictionary(x => x.Name.String, x => x.Value);
    }

    public class DnlibCustomAttributeBulder : DnlibCustomAttribute, ICustomAttributeBuilder
    {
        public DnlibCustomAttributeBulder(DnlibTypeSystem ts, CustomAttribute ca) : base(ts, ca)
        {
        }

        public void SetParameters(params object[] args)
        {
            foreach (var arg in args)
            {
                var dnlibType = (DnlibType) TypeSystem.FindType(arg.GetType());
                var caArg = new CAArgument(dnlibType.TypeRef.ToTypeSig(), arg);
                CustomAttribute.ConstructorArguments.Add(caArg);
            }
        }

        public void SetNamedProperties(Dictionary<string, object> props)
        {
            throw new NotImplementedException();
        }
    }
}