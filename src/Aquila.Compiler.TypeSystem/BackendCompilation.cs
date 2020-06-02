using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Aqua.TypeSystem.Builders;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua
{
    public class BackendCompilation
    {
        public void MakeMiracle(TypeManager tm, IAssemblyBuilder ab)
        {
            //Get all types for code building
            var typeForConstruct = tm.Types
                .Where(x => x.Scope.HasFlag(ScopeAffects.Code))
                .Where(x => x is PTypeBuilder || x is PTypeSpec)
                .OrderBy(x => x.BaseId)
                .Cast<PType>()
                .ToList();

            var ts = ab.TypeSystem;
            var sb = ts.GetSystemBindings();

            foreach (var type in typeForConstruct)
            {
                IType baseType;
                PType pBaseType = (PType) type.GetBase();

                if (pBaseType != tm.Unknown)
                    baseType = pBaseType.BackendType;
                else
                    baseType = sb.Object;

                //TODO: need construct TypeSpec

                var clrType = ab.DefineType(type.Name, type.Name, TypeAttributes.Abstract, baseType);
                type.BackendType = clrType;
            }

            foreach (var pType in typeForConstruct)
            {
                //TODO: construct fields
            }
        }
    }
}