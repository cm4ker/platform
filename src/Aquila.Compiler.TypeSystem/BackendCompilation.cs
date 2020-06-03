using System;
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

            //Step 1. Construct types only
            foreach (var type in typeForConstruct)
            {
                ConstructType(tm, ab, type, sb);
            }

            //Step 2. Construct members
            foreach (var pType in typeForConstruct)
            {
                foreach (var member in pType.Members)
                {
                    switch (member)
                    {
                        case PField f:
                            ConstructField(pType, f);
                            break;
                        case PProperty p:
                            ConstructProperty(pType, p);
                            break;
                        case PMethod m:
                            ConstructMethod(pType, m);
                            break;
                    }
                }
            }

            //Step 3 Construct bodies

            foreach (var pType in typeForConstruct)
            {
                foreach (var member in pType.Members)
                {
                    switch (member)
                    {
                        case PProperty p:
                            ConstructProperty(pType, p);
                            break;
                        case PMethod m:
                            EmitMethod((PMethodBuilder) m, (IMethodBuilder) m.BackendMethod);
                            break;
                    }
                }
            }
        }

        private static void ConstructType(TypeManager tm, IAssemblyBuilder ab, PType type, SystemTypeBindings sb)
        {
            if (type.BackendType != null)
                return;

            if (type is PTypeBuilder tb)
            {
                IType baseType;
                PType pBaseType = (PType) type.GetBase();

                if (pBaseType != tm.Unknown)
                {
                    if (pBaseType.BackendType == null)
                        ConstructType(tm, ab, pBaseType, sb);

                    baseType = pBaseType.BackendType;
                }
                else
                    baseType = sb.Object;

                //TODO: need construct TypeSpec

                var clrType = ab.DefineType(type.Name, type.Name, TypeAttributes.Abstract, baseType);
                clrType.DefineGenericParameters();
                
                type.BackendType = clrType;
            }
            else if (type is PTypeSpec ts)
            {
                if (ts.IsArray)
                {
                    var bType = (PType) ts.GetBase();
                    ts.BackendType = bType.BackendType.MakeArrayType(ts.Dimensions);
                }

                //TODO: If generic type
            }
        }

        private void ConstructField(PType type, PField field)
        {
            var tb = (ITypeBuilder) type.BackendType;
            field.BackendField = tb.DefineField(field.Type.ToBackend(), field.Name, true, false);
        }

        private void ConstructProperty(PType type, PProperty property)
        {
            var tb = (ITypeBuilder) type.BackendType;
            var backendProp = tb.DefineProperty(property.Type.ToBackend(), property.Name, false);

            backendProp.WithGetter(ConstructMethod(type, (PMethod) property.Getter));
            backendProp.WithSetter(ConstructMethod(type, (PMethod) property.Setter));

            property.BackendProperty = backendProp;
        }

        private IMethod ConstructMethod(PType type, PMethod method)
        {
            var tb = (ITypeBuilder) type.BackendType;
            var backendMethod = tb.DefineMethod(method.Name, true, false, false);
            backendMethod.WithReturnType(method.ReturnType.ToBackend());

            foreach (var pParameter in method.Parameters)
            {
                var pParam = (PParameter) pParameter;
                pParam.BackendParameter =
                    backendMethod.DefineParameter(pParameter.Name, pParam.Type.ToBackend(), false, false);
            }

            method.BackendMethod = backendMethod;
            return backendMethod;
        }

        private void EmitMethod(PMethodBuilder builder, IMethodBuilder backendBuilder)
        {
            var g = backendBuilder.Generator;

            foreach (var inst in builder.Body.Instructions)
            {
                var op = inst.OpCode;

                foreach (var local in builder.Body.Locals)
                {
                    local.BackendLocal = g.DefineLocal(local.Type.ToBackend());
                }

                foreach (var label in builder.Body.Labels)
                {
                    label.BackendLabel = g.DefineLabel();
                }

                switch (inst.Operand)
                {
                    case int i:
                        g.Emit(op, i);
                        break;
                    case double i:
                        g.Emit(op, i);
                        break;
                    case string i:
                        g.Emit(op, i);
                        break;
                    case long i:
                        g.Emit(op, i);
                        break;
                    case PType i:
                        g.Emit(op, i.BackendType);
                        break;
                    case PField i:
                        g.Emit(op, i.BackendField);
                        break;
                    case PMethod i:
                        g.Emit(op, i.BackendMethod);
                        break;
                    case PConstructor i:
                        g.Emit(op, i.BackendConstructor);
                        break;
                    case PParameter i:
                        g.Emit(op, i.BackendParameter);
                        break;
                    case PLocal i:
                        g.Emit(op, i.BackendLocal);
                        break;
                    case PInstruction i:
                        g.Emit(op, ((PLabel) i.Operand).BackendLabel);
                        break;
                    case PLabel i:
                        g.MarkLabel(i.BackendLabel);
                        break;
                    default:
                        throw new Exception("Not supported");
                }
            }
        }
    }
}