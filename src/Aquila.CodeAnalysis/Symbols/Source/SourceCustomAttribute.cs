﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Expressions;
using Aquila.Syntax.Syntax;
using Microsoft.CodeAnalysis;
using Pchp.CodeAnalysis;
using Peachpie.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.Symbols.Source
{
    sealed class SourceCustomAttribute : BaseAttributeData
    {
        readonly TypeRef _tref;
        readonly ImmutableArray<LangElement> _arguments;
        readonly ImmutableArray<KeyValuePair<Name, LangElement>> _properties;

        NamedTypeSymbol _type;
        MethodSymbol _ctor;
        ImmutableArray<TypedConstant> _ctorArgs;
        ImmutableArray<KeyValuePair<string, TypedConstant>> _namedArgs;

        public SourceCustomAttribute(TypeRef tref, IList<KeyValuePair<Name, LangElement>> arguments)
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
                _arguments = ImmutableArray<LangElement>.Empty;
                _properties = ImmutableArray<KeyValuePair<Name, LangElement>>.Empty;
            }
        }

        #region Bind to Symbol and TypedConstant

        internal void Bind(Symbol symbol, SourceFileSymbol file)
        {
            Debug.Assert(symbol != null);

            if (_type == null)
            {
                // TODO: check the attribute can bi bound to symbol

                var type = (NamedTypeSymbol) null; //symbol.DeclaringCompilation.GetTypeFromTypeRef(_tref);

                if (type.IsErrorTypeOrNull() || type.SpecialType == SpecialType.System_Object)
                {
                    DiagnosticBagExtensions.Add(symbol.DeclaringCompilation.DeclarationDiagnostics,
                        Location.Create(file.SyntaxTree, _tref.Span.ToTextSpan()),
                        Errors.ErrorCode.ERR_TypeNameCannotBeResolved,
                        _tref.ToString());

                    type = new MissingMetadataTypeSymbol(_tref.ToString(), 0, false);
                }

                // bind arguments
                if (!TryResolveCtor(type, symbol.DeclaringCompilation, out _ctor, out _ctorArgs) && type.IsValidType())
                {
                    DiagnosticBagExtensions.Add(symbol.DeclaringCompilation.DeclarationDiagnostics,
                        Location.Create(file.SyntaxTree, _tref.Span.ToTextSpan()),
                        Errors.ErrorCode.ERR_NoMatchingOverload,
                        type.Name + "..ctor");
                }

                // bind named parameters
                if (type.IsErrorTypeOrNull() || _properties.IsDefaultOrEmpty)
                {
                    _namedArgs = ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty;
                }
                else
                {
                    var namedArgs = new KeyValuePair<string, TypedConstant>[_properties.Length];
                    for (int i = 0; i < namedArgs.Length; i++)
                    {
                        var prop = _properties[i];
                        var member =
                            (Symbol) type.LookupMember<PropertySymbol>(prop.Key.Value) ??
                            (Symbol) type.LookupMember<FieldSymbol>(prop.Key.Value);

                        if (member != null && TryBindTypedConstant(member.GetTypeOrReturnType(), prop.Value,
                            symbol.DeclaringCompilation, out var arg))
                        {
                            namedArgs[i] = new KeyValuePair<string, TypedConstant>(prop.Key.Value, arg);
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }

                    _namedArgs = namedArgs.AsImmutable();
                }

                //
                _type = type;
            }
        }

        static bool TryBindTypedConstant(TypeSymbol target, long value, out TypedConstant result)
        {
            switch (target.SpecialType)
            {
                case SpecialType.System_Byte:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (byte) value);
                    return true;
                case SpecialType.System_Int32:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (int) value);
                    return true;
                case SpecialType.System_Int64:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, value);
                    return true;
                case SpecialType.System_UInt32:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (uint) value);
                    return true;
                case SpecialType.System_UInt64:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (ulong) value);
                    return true;
                case SpecialType.System_Double:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (double) value);
                    return true;
                case SpecialType.System_Single:
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (float) value);
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
                    result = new TypedConstant(target, TypedConstantKind.Primitive, (float) value);
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
            Debug.Assert(!(value is LangElement));

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

        static bool TryBindTypedConstant(TypeSymbol target, LangElement element, PhpCompilation compilation,
            out TypedConstant result)
        {
            if (element is LiteralEx lit)
            {
                return TryBindTypedConstant(target, lit.Value, out result);
            }

            if (element is TypeRef tref)
            {
                var system_type = compilation.GetWellKnownType(WellKnownType.System_Type);
                result = new TypedConstant(system_type, TypedConstantKind.Type, null); //compilation.GetTypeFromTypeRef(tref));
                return target == system_type;
            }

            // if (element is GlobalConstUse gconst)
            // {
            //     var qname = gconst.FullName.Name.QualifiedName;
            //     if (qname.IsSimpleName)
            //     {
            //         // common constants
            //         if (qname == QualifiedName.True) return TryBindTypedConstant(target, true, out result);
            //         if (qname == QualifiedName.False) return TryBindTypedConstant(target, true, out result);
            //         if (qname == QualifiedName.Null) return TryBindTypedConstant(target, (object) null, out result);
            //
            //         // lookup constant
            //         var csymbol = compilation.GlobalSemantics.ResolveConstant(qname.Name.Value);
            //         if (csymbol is FieldSymbol fld && fld.HasConstantValue)
            //         {
            //             return TryBindTypedConstant(target, fld.ConstantValue, out result);
            //         }
            //     }
            //
            //     // note: namespaced constants are unreachable
            // }

            // if (element is ClassConstUse cconst)
            // {
            //     // lookup the type container
            //     var ctype = compilation.GetTypeFromTypeRef(cconst.TargetType);
            //     if (ctype.IsValidType())
            //     {
            //         // lookup constant/enum field (both are FieldSymbol)
            //         var member = ctype.LookupMember<FieldSymbol>(cconst.Name.Value);
            //         if (member != null && member.HasConstantValue)
            //         {
            //             return TryBindTypedConstant(target, member.ConstantValue, out result);
            //         }
            //     }
            // }

            //
            result = default;
            return false;
        }

        bool TryResolveCtor(NamedTypeSymbol type, PhpCompilation compilation, out MethodSymbol ctor,
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
                            //if (ps[pi].IsOptional)
                            //{
                            //    boundargs[pi] = ps[pi].ExplicitDefaultConstantValue.AsTypedConstant();
                            //    continue; // ok
                            //}
                            //else
                            {
                                match = false;
                                break;
                            }
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