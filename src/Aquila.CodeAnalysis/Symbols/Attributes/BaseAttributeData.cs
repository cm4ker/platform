using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Symbols.Source;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Represents an attribute applied to a Symbol.
    /// </summary>
    internal abstract partial class BaseAttributeData : AttributeData
    {
        /// <summary>
        /// Gets the attribute class being applied.
        /// </summary>
        public new abstract NamedTypeSymbol AttributeClass { get; }

        /// <summary>
        /// Gets the constructor used in this application of the attribute.
        /// </summary>
        public new abstract MethodSymbol AttributeConstructor { get; }

        /// <summary>
        /// Gets a reference to the source for this application of the attribute. Returns null for applications of attributes on metadata Symbols.
        /// </summary>
        public new abstract SyntaxReference ApplicationSyntaxReference { get; }

        /// <summary>
        /// Gets the list of constructor arguments specified by this application of the attribute.  This list contains both positional arguments
        /// and named arguments that are formal parameters to the constructor.
        /// </summary>
        public new IEnumerable<TypedConstant> ConstructorArguments
        {
            get { return this.CommonConstructorArguments; }
        }

        /// <summary>
        /// Gets the list of named field or property value arguments specified by this application of the attribute.
        /// </summary>
        public new IEnumerable<KeyValuePair<string, TypedConstant>> NamedArguments
        {
            get { return this.CommonNamedArguments; }
        }

        /// <summary>
        /// Compares the namespace and type name with the attribute's namespace and type name.
        /// Returns true if they are the same.
        /// </summary>
        internal virtual bool IsTargetAttribute(string namespaceName, string typeName)
        {
            if (!this.AttributeClass.Name.Equals(typeName))
            {
                return false;
            }

            if (this.AttributeClass.IsErrorType() && !(this.AttributeClass is MissingMetadataTypeSymbol))
            {
                // Can't guarantee complete name information.
                return false;
            }

            return this.AttributeClass.HasNameQualifier(namespaceName);
        }

        internal bool IsTargetAttribute(Symbol targetSymbol, AttributeDescription description)
        {
            return GetTargetAttributeSignatureIndex(targetSymbol, description) != -1;
        }

        internal abstract int GetTargetAttributeSignatureIndex(Symbol targetSymbol, AttributeDescription description);

        /// <summary>
        /// Returns the <see cref="System.String"/> that represents the current AttributeData.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current AttributeData.</returns>
        public override string ToString()
        {
            if ((object)this.AttributeClass != null)
            {
                string className = this.AttributeClass.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat);

                if (!this.CommonConstructorArguments.Any() & !this.CommonNamedArguments.Any())
                {
                    return className;
                }

                var pooledStrbuilder = PooledStringBuilder.GetInstance();
                StringBuilder stringBuilder = pooledStrbuilder.Builder;

                stringBuilder.Append(className);
                stringBuilder.Append("(");

                bool first = true;

                foreach (var constructorArgument in this.CommonConstructorArguments)
                {
                    if (!first)
                    {
                        stringBuilder.Append(", ");
                    }

                    stringBuilder.Append(constructorArgument.ToCSharpString());
                    first = false;
                }

                foreach (var namedArgument in this.CommonNamedArguments)
                {
                    if (!first)
                    {
                        stringBuilder.Append(", ");
                    }

                    stringBuilder.Append(namedArgument.Key);
                    stringBuilder.Append(" = ");
                    stringBuilder.Append(namedArgument.Value.ToCSharpString());
                    first = false;
                }

                stringBuilder.Append(")");

                return pooledStrbuilder.ToStringAndFree();
            }

            return base.ToString();
        }

        #region AttributeData Implementation

        /// <summary>
        /// Gets the attribute class being applied as an <see cref="INamedTypeSymbol"/>
        /// </summary>
        protected override INamedTypeSymbol CommonAttributeClass
        {
            get { return this.AttributeClass; }
        }

        /// <summary>
        /// Gets the constructor used in this application of the attribute as an <see cref="IMethodSymbol"/>.
        /// </summary>
        protected override IMethodSymbol CommonAttributeConstructor
        {
            get { return this.AttributeConstructor; }
        }

        /// <summary>
        /// Gets a reference to the source for this application of the attribute. Returns null for applications of attributes on metadata Symbols.
        /// </summary>
        protected override SyntaxReference CommonApplicationSyntaxReference
        {
            get { return this.ApplicationSyntaxReference; }
        }

        #endregion

        #region Attribute Decoding

        // This method checks if the given PermissionSetAttribute type has a property member with the given propName which is writable, non-generic, public and of string type.
        private static bool PermissionSetAttributeTypeHasRequiredProperty(NamedTypeSymbol permissionSetType,
            string propName)
        {
            var members = permissionSetType.GetMembers(propName);
            if (members.Length == 1 && members[0].Kind == SymbolKind.Property)
            {
                var property = (PropertySymbol)members[0];
                if ((object)property.Type != null && property.Type.SpecialType == SpecialType.System_String &&
                    property.DeclaredAccessibility == Accessibility.Public && property.GetMemberArity() == 0 &&
                    (object)property.SetMethod != null &&
                    property.SetMethod.DeclaredAccessibility == Accessibility.Public)
                {
                    return true;
                }
            }

            return false;
        }

        internal void DecodeClassInterfaceAttribute(SyntaxNode nodeOpt, DiagnosticBag diagnostics)
        {
            Debug.Assert(!this.HasErrors);

            //TypedConstant ctorArgument = this.CommonConstructorArguments[0];
            //Debug.Assert(ctorArgument.Kind == TypedConstantKind.Enum || ctorArgument.Kind == TypedConstantKind.Primitive);

            //ClassInterfaceType interfaceType = ctorArgument.Kind == TypedConstantKind.Enum ?
            //    ctorArgument.DecodeValue<ClassInterfaceType>(SpecialType.System_Enum) :
            //    (ClassInterfaceType)ctorArgument.DecodeValue<short>(SpecialType.System_Int16);

            //switch (interfaceType)
            //{
            //    case ClassInterfaceType.None:
            //    case ClassInterfaceType.AutoDispatch:
            //    case ClassInterfaceType.AutoDual:
            //        break;

            //    default:
            //        throw new NotImplementedException();
            //        //// CS0591: Invalid value for argument to '{0}' attribute
            //        //Location attributeArgumentSyntaxLocation = this.GetAttributeArgumentSyntaxLocation(0, nodeOpt);
            //        //diagnostics.Add(ErrorCode.ERR_InvalidAttributeArgument, attributeArgumentSyntaxLocation, nodeOpt != null ? nodeOpt.GetErrorDisplayName() : "");
            //        //break;
            //}
        }

        internal void DecodeInterfaceTypeAttribute(SyntaxNode node, DiagnosticBag diagnostics)
        {
            Debug.Assert(!this.HasErrors);

            //TypedConstant ctorArgument = this.CommonConstructorArguments[0];
            //Debug.Assert(ctorArgument.Kind == TypedConstantKind.Enum || ctorArgument.Kind == TypedConstantKind.Primitive);

            //ComInterfaceType interfaceType = ctorArgument.Kind == TypedConstantKind.Enum ?
            //    ctorArgument.DecodeValue<ComInterfaceType>(SpecialType.System_Enum) :
            //    (ComInterfaceType)ctorArgument.DecodeValue<short>(SpecialType.System_Int16);

            //switch (interfaceType)
            //{
            //    case ComInterfaceType.InterfaceIsDual:
            //    case ComInterfaceType.InterfaceIsIDispatch:
            //    case ComInterfaceType.InterfaceIsIInspectable:
            //    case ComInterfaceType.InterfaceIsIUnknown:
            //        break;

            //    default:
            //        throw new NotImplementedException();
            //        //// CS0591: Invalid value for argument to '{0}' attribute
            //        //CSharpSyntaxNode attributeArgumentSyntax = this.GetAttributeArgumentSyntax(0, node);
            //        //diagnostics.Add(ErrorCode.ERR_InvalidAttributeArgument, attributeArgumentSyntax.Location, node.GetErrorDisplayName());
            //        //break;
            //}
        }

        internal string DecodeGuidAttribute(SyntaxNode nodeOpt, DiagnosticBag diagnostics)
        {
            Debug.Assert(!this.HasErrors);

            var guidString = (string)this.CommonConstructorArguments[0].Value;

            // Native compiler allows only a specific GUID format: "D" format (32 digits separated by hyphens)
            Guid guid;
            if (!Guid.TryParseExact(guidString, "D", out guid))
            {
                throw new NotImplementedException();
                //// CS0591: Invalid value for argument to '{0}' attribute
                //Location attributeArgumentSyntaxLocation = this.GetAttributeArgumentSyntaxLocation(0, nodeOpt);
                //diagnostics.Add(ErrorCode.ERR_InvalidAttributeArgument, attributeArgumentSyntaxLocation, nodeOpt != null ? nodeOpt.GetErrorDisplayName() : "");
                //guidString = String.Empty;
            }

            return guidString;
        }

        #endregion

        /// <summary>
        /// This method determines if an applied attribute must be emitted.
        /// Some attributes appear in symbol model to reflect the source code,
        /// but should not be emitted.
        /// </summary>
        internal bool ShouldEmitAttribute(Symbol target, bool isReturnType, bool emittingAssemblyAttributesInNetModule)
        {
            Debug.Assert(target is SourceAssemblySymbol || target.ContainingAssembly is SourceAssemblySymbol);

            if (HasErrors)
            {
                throw ExceptionUtilities.Unreachable;
            }

            // Attribute type is conditionally omitted if both the following are true:
            //  (a) It has at least one applied/inherited conditional attribute AND
            //  (b) None of conditional symbols are defined in the source file where the given attribute was defined.
            if (this.IsConditionallyOmitted)
            {
                return false;
            }

            switch (target.Kind)
            {
                case SymbolKind.Assembly:
                    if ((!emittingAssemblyAttributesInNetModule &&
                         (IsTargetAttribute(target, AttributeDescription.AssemblyCultureAttribute) ||
                          IsTargetAttribute(target, AttributeDescription.AssemblyVersionAttribute) ||
                          IsTargetAttribute(target, AttributeDescription.AssemblyFlagsAttribute) ||
                          IsTargetAttribute(target, AttributeDescription.AssemblyAlgorithmIdAttribute))) ||
                        IsTargetAttribute(target, AttributeDescription.TypeForwardedToAttribute) ||
                        false) // IsSecurityAttribute(target.DeclaringCompilation))
                    {
                        return false;
                    }

                    break;

                case SymbolKind.Event:
                    if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute))
                    {
                        return false;
                    }

                    break;

                case SymbolKind.Field:
                    if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) ||
                        IsTargetAttribute(target, AttributeDescription.NonSerializedAttribute) ||
                        IsTargetAttribute(target, AttributeDescription.FieldOffsetAttribute) ||
                        IsTargetAttribute(target, AttributeDescription.MarshalAsAttribute))
                    {
                        return false;
                    }

                    break;

                case SymbolKind.Method:
                    if (isReturnType)
                    {
                        if (IsTargetAttribute(target, AttributeDescription.MarshalAsAttribute))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) ||
                            IsTargetAttribute(target, AttributeDescription.MethodImplAttribute) ||
                            IsTargetAttribute(target, AttributeDescription.DllImportAttribute) ||
                            IsTargetAttribute(target, AttributeDescription.PreserveSigAttribute) ||
                            IsTargetAttribute(target, AttributeDescription.DynamicSecurityMethodAttribute) ||
                            false) // IsSecurityAttribute(target.DeclaringCompilation))
                        {
                            return false;
                        }
                    }

                    break;

                case SymbolKind.NetModule:
                    // Note that DefaultCharSetAttribute is emitted to metadata, although it's also decoded and used when emitting P/Invoke
                    break;

                case SymbolKind.NamedType:
                    if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) ||
                        IsTargetAttribute(target, AttributeDescription.ComImportAttribute) ||
                        IsTargetAttribute(target, AttributeDescription.SerializableAttribute) ||
                        IsTargetAttribute(target, AttributeDescription.StructLayoutAttribute) ||
                        IsTargetAttribute(target, AttributeDescription.WindowsRuntimeImportAttribute) ||
                        false) // IsSecurityAttribute(target.DeclaringCompilation))
                    {
                        return false;
                    }

                    break;

                case SymbolKind.Parameter:
                    if (IsTargetAttribute(target, AttributeDescription.OptionalAttribute) ||
                        IsTargetAttribute(target, AttributeDescription.DefaultParameterValueAttribute) ||
                        IsTargetAttribute(target, AttributeDescription.InAttribute) ||
                        IsTargetAttribute(target, AttributeDescription.OutAttribute) ||
                        IsTargetAttribute(target, AttributeDescription.MarshalAsAttribute))
                    {
                        return false;
                    }

                    break;

                case SymbolKind.Property:
                    if (IsTargetAttribute(target, AttributeDescription.IndexerNameAttribute) ||
                        IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute))
                    {
                        return false;
                    }

                    break;
            }

            return true;
        }
    }

    internal static class AttributeDataExtensions
    {
        internal static int IndexOfAttribute(this ImmutableArray<BaseAttributeData> attributes, Symbol targetSymbol,
            AttributeDescription description)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i].IsTargetAttribute(targetSymbol, description))
                {
                    return i;
                }
            }

            return -1;
        }

        internal static SyntaxNode GetAttributeArgumentSyntax(this AttributeData attribute, int parameterIndex,
            SyntaxNode attributeSyntax)
        {
            Debug.Assert(attribute is SourceAttributeData);
            return ((SourceAttributeData)attribute).GetAttributeArgumentSyntax(parameterIndex, attributeSyntax);
        }

        internal static Location GetAttributeArgumentSyntaxLocation(this AttributeData attribute, int parameterIndex,
            SyntaxNode attributeSyntaxOpt)
        {
            if (attributeSyntaxOpt == null)
            {
                return NoLocation.Singleton;
            }

            throw new NotImplementedException();
            //Debug.Assert(attribute is SourceAttributeData);
            //return ((SourceAttributeData)attribute).GetAttributeArgumentSyntax(parameterIndex, attributeSyntaxOpt).Location;
        }
    }
}