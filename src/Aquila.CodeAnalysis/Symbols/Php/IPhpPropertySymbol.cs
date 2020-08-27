using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.CodeGen;

namespace Aquila.CodeAnalysis.Symbols.Php
{
    /// <summary>
    /// The field kind.
    /// </summary>
    public enum PhpPropertyKind
    {
        InstanceField,
        StaticField,
        AppStaticField,
        ClassConstant,
    }

    /// <summary>
    /// Describes a PHP property.
    /// </summary>
    interface IPhpPropertySymbol : IPhpValue
    {
        /// <summary>
        /// PHP property kind.
        /// </summary>
        PhpPropertyKind FieldKind { get; }

        /// <summary>
        /// In case field is contained in <c>__statics</c> holder class, gets its type.
        /// Otherwise <c>null</c>.
        /// </summary>
        TypeSymbol ContainingStaticsHolder { get; }

        /// <summary>
        /// Whether initialization of the field requires reference to runtime context.
        /// </summary>
        bool RequiresContext { get; }

        /// <summary>
        /// Type declaring this PHP property.
        /// </summary>
        TypeSymbol DeclaringType { get; }

        /// <summary>
        /// Emits initialization of the field.
        /// </summary>
        /// <param name="cg"></param>
        void EmitInit(CodeGenerator cg);

        /// <summary>
        /// PHP property visibility.
        /// </summary>
        Accessibility DeclaredAccessibility { get; }

        /// <summary>
        /// PHP property name,
        /// </summary>
        string Name { get; }
    }
}
