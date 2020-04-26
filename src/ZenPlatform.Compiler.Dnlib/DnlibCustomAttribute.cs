using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IAssembly = dnlib.DotNet.IAssembly;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
using IMethod = dnlib.DotNet.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibCustomAttribute : ICustomAttribute
    {
        private readonly DnlibTypeSystem _ts;
        private CustomAttribute _ca;

        public DnlibCustomAttribute(DnlibTypeSystem ts, CustomAttribute ca)
        {
            _ts = ts;
            _ca = ca;
        }

        protected IMethod Constructor { get; set; }

        protected DnlibContextResolver Resolver { get; }

        internal DnlibTypeSystem TypeSystem => _ts;

        protected CustomAttribute CustomAttribute
        {
            get { return _ca; }
            set
            {
                _ca = value;
                Constructor = _ca.Constructor;
            }
        }

        public IType AttributeType => (_ca != null) ? _ts.Resolve(_ca.AttributeType) : null;

        public CustomAttribute GetCA() => _ca;

        public bool Equals(ICustomAttribute other)
        {
            if (other is DnlibCustomAttribute a)
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

    public class DnlibCustomAttributeBulder : DnlibCustomAttribute, ICustomAttributeBuilder
    {
        public DnlibCustomAttributeBulder(DnlibTypeSystem ts, IMethod constructor) : base(ts, null)
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
                var dnlibType = (DnlibType) TypeSystem.FindType(arg.GetType());
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