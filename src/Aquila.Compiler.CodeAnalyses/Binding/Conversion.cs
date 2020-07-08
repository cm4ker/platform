using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class Conversion
    {
        public static readonly Conversion None = new Conversion(exists: false, isIdentity: false, isImplicit: false);
        public static readonly Conversion Identity = new Conversion(exists: true, isIdentity: true, isImplicit: true);
        public static readonly Conversion Implicit = new Conversion(exists: true, isIdentity: false, isImplicit: true);
        public static readonly Conversion Explicit = new Conversion(exists: true, isIdentity: false, isImplicit: false);

        private Conversion(bool exists, bool isIdentity, bool isImplicit)
        {
            Exists = exists;
            IsIdentity = isIdentity;
            IsImplicit = isImplicit;
        }

        public bool Exists { get; }
        public bool IsIdentity { get; }
        public bool IsImplicit { get; }
        public bool IsExplicit => Exists && !IsImplicit;

        public static Conversion Classify(NamedTypeSymbol from, NamedTypeSymbol to)
        {
            if (from == to)
                return Conversion.Identity;
            //
            // if (from != NamedTypeSymbol.Void && to == NamedTypeSymbol.Any)
            // {
            //     return Conversion.Implicit;
            // }
            //
            // if (from == NamedTypeSymbol.Any && to != NamedTypeSymbol.Void)
            // {
            //     return Conversion.Explicit;
            // }
            //
            // if (from == NamedTypeSymbol.Bool || from == NamedTypeSymbol.Int)
            // {
            //     if (to == NamedTypeSymbol.String)
            //         return Conversion.Explicit;
            // }
            //
            // if (from == NamedTypeSymbol.String)
            // {
            //     if (to == NamedTypeSymbol.Bool || to == NamedTypeSymbol.Int)
            //         return Conversion.Explicit;
            // }

            return Conversion.None;
        }
    }
}
