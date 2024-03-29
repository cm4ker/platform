﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Semantics;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.Symbols.Source
{
    sealed class SourceCustomAttribute : BaseAttributeData
    {
        readonly NameEx _tref;
        readonly ImmutableArray<AquilaSyntaxNode> _arguments;
        readonly ImmutableArray<KeyValuePair<Name, AquilaSyntaxNode>> _properties;

        NamedTypeSymbol _type;
        MethodSymbol _ctor;
        ImmutableArray<TypedConstant> _ctorArgs;
        ImmutableArray<KeyValuePair<string, TypedConstant>> _namedArgs;

        public SourceCustomAttribute(NameEx tref, IList<KeyValuePair<Name, AquilaSyntaxNode>> arguments)
        {
            _tref = tref;

            if (arguments != null && arguments.Count != 0)
            {
                // count args
                int nargs = 0;
                while (nargs < arguments.Count && arguments[nargs].Key.Value == null)
                    nargs++;

                //
                _arguments = arguments.Take(nargs).Select(x => x.Value).AsImmutable();
                _properties = arguments.Skip(nargs).AsImmutable();
            }
            else
            {
                _arguments = ImmutableArray<AquilaSyntaxNode>.Empty;
                _properties = ImmutableArray<KeyValuePair<Name, AquilaSyntaxNode>>.Empty;
            }
        }

        #region Bind to Symbol and TypedConstant

        static bool TryBindTypedConstant(TypeSymbol target, long value, out TypedConstant result)
        {
            switch (target.SpecialType)
            {
                case SpecialType.System_Byte:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (byte)value);
                    return true;
                case SpecialType.System_Int32:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (int)value);
                    return true;
                case SpecialType.System_Int64:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, value);
                    return true;
                case SpecialType.System_UInt32:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (uint)value);
                    return true;
                case SpecialType.System_UInt64:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (ulong)value);
                    return true;
                case SpecialType.System_Double:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (double)value);
                    return true;
                case SpecialType.System_Single:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (float)value);
                    return true;
                default:

                    if (target.IsEnumType())
                    {
                        return TryBindTypedConstant(target.GetEnumUnderlyingType(), value, out result);
                    }

                    result = default;
                    return false;
            }
        }

        static bool TryBindTypedConstant(TypeSymbol target, double value, out TypedConstant result)
        {
            switch (target.SpecialType)
            {
                case SpecialType.System_Double:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, value);
                    return true;
                case SpecialType.System_Single:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (float)value);
                    return true;
                default:
                    result = default;
                    return false;
            }
        }

        static bool TryBindTypedConstant(TypeSymbol target, string value, out TypedConstant result)
        {
            switch (target.SpecialType)
            {
                case SpecialType.System_String:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, value);
                    return true;
                case SpecialType.System_Char:
                    if (value.Length != 1) goto default;
                    result = new TypedConstant(target, TypedConstantKind.Primitive, value[0]);
                    return true;
                default:
                    result = default;
                    return false;
            }
        }

        static bool TryBindTypedConstant(TypeSymbol target, bool value, out TypedConstant result)
        {
            switch (target.SpecialType)
            {
                case SpecialType.System_Boolean:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, value);
                    return true;
                default:
                    result = default;
                    return false;
            }
        }

        static bool TryBindTypedConstant(TypeSymbol target, object value, out TypedConstant result)
        {
            Debug.Assert(!(value is AquilaSyntaxNode));

            if (ReferenceEquals(value, null))
            {
                // NULL
                result = new TypedConstant(target, TypedConstantKind.Primitive, null);
                return true && target.IsReferenceType;
            }

            if (value is int i) return TryBindTypedConstant(target, i, out result);
            if (value is long l) return TryBindTypedConstant(target, l, out result);
            if (value is uint u) return TryBindTypedConstant(target, u, out result);
            if (value is byte b8) return TryBindTypedConstant(target, b8, out result);
            if (value is double d) return TryBindTypedConstant(target, d, out result);
            if (value is float f) return TryBindTypedConstant(target, f, out result);
            if (value is string s) return TryBindTypedConstant(target, s, out result);
            if (value is char c) return TryBindTypedConstant(target, c.ToString(), out result);
            if (value is bool b) return TryBindTypedConstant(target, b, out result);

            //
            result = default;
            return false;
        }

        static bool TryBindTypedConstant(TypeSymbol target, AquilaSyntaxNode element, AquilaCompilation compilation,
            out TypedConstant result)
        {
            if (element is LiteralEx lit)
            {
                throw new NotImplementedException();
            }

            if (element is TypeEx tref)
            {
                var system_type = compilation.GetWellKnownType(WellKnownType.System_Type);
                result = new TypedConstant(system_type, TypedConstantKind.Type,
                    null);
                return target == system_type;
            }

            result = default;
            return false;
        }

        bool TryResolveCtor(NamedTypeSymbol type, AquilaCompilation compilation, out MethodSymbol ctor,
            out ImmutableArray<TypedConstant> args)
        {
            if (type.IsValidType())
            {
                var candidates = type.InstanceConstructors;
                for (int i = 0; i < candidates.Length; i++)
                {
                    var m = candidates[i];

                    if (m.DeclaredAccessibility != Accessibility.Public) continue; // TODO: or current class context
                    if (m.IsGenericMethod)
                    {
                        Debug.Fail("unexpected");
                        continue;
                    } // NS

                    if (m.ParameterCount < _arguments.Length) continue; // be strict

                    var match = true;
                    var ps = m.Parameters;
                    var boundargs = new TypedConstant[ps.Length];

                    for (var pi = 0; match && pi < ps.Length; pi++)
                    {
                        if (pi >= _arguments.Length)
                        {
                            match = false;
                            break;
                        }

                        if (TryBindTypedConstant(ps[pi].Type, _arguments[pi], compilation, out var arg))
                        {
                            boundargs[pi] = arg;
                        }
                        else
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        ctor = m;
                        args = boundargs.AsImmutable();
                        return true;
                    }
                }
            }

            //
            ctor = new MissingMethodSymbol();
            args = ImmutableArray<TypedConstant>.Empty;
            return false;
        }

        #endregion

        public override NamedTypeSymbol AttributeClass => _type ?? throw ExceptionUtilities.Unreachable;

        public override MethodSymbol AttributeConstructor => _ctor ?? throw ExceptionUtilities.Unreachable;

        public override SyntaxReference ApplicationSyntaxReference => throw new NotImplementedException();

        protected internal override ImmutableArray<TypedConstant> CommonConstructorArguments => _ctorArgs;

        protected internal override ImmutableArray<KeyValuePair<string, TypedConstant>> CommonNamedArguments =>
            _namedArgs;

        internal override int GetTargetAttributeSignatureIndex(Symbol targetSymbol, AttributeDescription description)
        {
            throw new NotImplementedException();
        }
    }
}