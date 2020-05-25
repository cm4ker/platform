using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynCustomAttribute : Contracts.ICustomAttribute
    {
        private readonly RoslynTypeSystem _ts;
        private CustomAttribute _ca;

        public RoslynCustomAttribute(RoslynTypeSystem ts, CustomAttribute ca)
        {
            _ts = ts;
            _ca = ca;
        }

        protected IMethod Constructor { get; set; }

        protected RoslynContextResolver Resolver { get; }

        internal RoslynTypeSystem TypeSystem => _ts;

        protected CustomAttribute CustomAttribute
        {
            get { return _ca; }
            set
            {
                _ca = value;
                Constructor = _ca.Constructor;
            }
        }

        public RoslynType AttributeType => (_ca != null) ? _ts.Resolve(_ca.AttributeType) : null;

        public CustomAttribute GetCA() => _ca;

        public bool Equals(ICustomAttribute other)
        {
            if (other is RoslynCustomAttribute a)
                return _ca.Equals(a._ca);

            return false;
        }

        internal void ImportAttribute(ModuleDef module)
        {
            var m = (MemberRef) module.Import(Constructor);

            if (CustomAttribute is null)
                CustomAttribute = new CustomAttribute(m);
            else
                CustomAttribute.Constructor = m;

            AfterImport();
        }

        protected virtual void AfterImport()
        {
        }


        public List<object> Parameters => _ca.ConstructorArguments.Select(x => x.Value).ToList();

        public Dictionary<string, object> Properties => _ca.Properties.ToDictionary(x => x.Name.String, x => x.Value);
    }
}