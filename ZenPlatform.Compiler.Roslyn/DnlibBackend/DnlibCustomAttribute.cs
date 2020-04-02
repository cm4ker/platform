using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SreCustomAttribute
    {
        private readonly SreTypeSystem _ts;
        private CustomAttribute _ca;

        public SreCustomAttribute(SreTypeSystem ts, CustomAttribute ca)
        {
            _ts = ts;
            _ca = ca;
        }

        protected IMethod Constructor { get; set; }

        protected SreContextResolver Resolver { get; }

        internal SreTypeSystem TypeSystem => _ts;

        protected CustomAttribute CustomAttribute
        {
            get { return _ca; }
            set
            {
                _ca = value;
                Constructor = _ca.Constructor;
            }
        }

        public SreType AttributeType => (_ca != null) ? _ts.Resolve(_ca.AttributeType) : null;

        public CustomAttribute GetCA() => _ca;

        public bool Equals(ICustomAttribute other)
        {
            if (other is SreCustomAttribute a)
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

    public class SreCustomAttributeBulder : SreCustomAttribute
    {
        public SreCustomAttributeBulder(SreTypeSystem ts, IMethod constructor) : base(ts, null)
        {
            Constructor = constructor;
        }

        private object[] _args;

        public void SetParameters(params object[] args)
        {
            _args = args;
            if (_args != null && CustomAttribute != null)
            {
                UpdateConstructorArguments();
            }
        }

        private void UpdateConstructorArguments()
        {
            foreach (var arg in _args)
            {
                var dnlibType = TypeSystem.Resolve(arg.GetType());
                var caArg = new CAArgument(dnlibType.TypeRef.ToTypeSig(), arg);
                CustomAttribute.ConstructorArguments.Add(caArg);
            }
        }

        protected override void AfterImport()
        {
            CustomAttribute.ConstructorArguments.Clear();

            if (_args != null)
                UpdateConstructorArguments();
        }

        public void SetNamedProperties(Dictionary<string, object> props)
        {
            throw new NotImplementedException();
        }
    }
}