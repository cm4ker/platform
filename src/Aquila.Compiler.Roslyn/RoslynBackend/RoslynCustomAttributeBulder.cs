using System;
using System.Collections.Generic;
using Aquila.Compiler.Contracts;
using dnlib.DotNet;
using IMethod = dnlib.DotNet.IMethod;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynCustomAttributeBulder : RoslynCustomAttribute, ICustomAttributeBuilder
    {
        public RoslynCustomAttributeBulder(RoslynTypeSystem ts, IMethod constructor) : base(ts, null)
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
                var dnlibType = (RoslynType) TypeSystem.Resolve(arg.GetType());
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