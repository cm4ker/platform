using Aquila.Language.Ast.Symbols.PE;

namespace Aquila.Language.Ast.Symbols
{
    public abstract class ParameterSymbol : Symbol
    {
        public virtual TypeSymbol Type => null;


        public override SymbolKind Kind => SymbolKind.Parameter;

        public virtual int Ordinal { get; }
    }


    public abstract class SourceParameterSymbolBase : ParameterSymbol
    {
    }

    public abstract class SourceParameterSymbol : SourceParameterSymbolBase
    {
        private readonly ModuleSymbol _moduleSymbol;
        private readonly MethodSymbol _methodSymbol;
        private readonly TypeSymbol _parameterType;
        private readonly ParameterDefinition _parameterDefinition;

        internal SourceParameterSymbol()
        {
        }
    }


    public sealed class SourceSimpleParameterSymbol : SourceParameterSymbol
    {
        public SourceSimpleParameterSymbol(
            string name,
            TypeSymbol parameterType,
            int ordinal
        )

        {
        }
    }

    public class PEParameterSymbol : ParameterSymbol
    {
        private readonly PEModuleSymbol _moduleSymbol;
        private readonly MethodSymbol _methodSymbol;
        private readonly TypeSymbol _parameterType;
        private readonly ParameterDefinition _parameterDefinition;

        internal PEParameterSymbol(PEModuleSymbol moduleSymbol, MethodSymbol methodSymbol, TypeSymbol parameterType,
            ParameterDefinition parameterDefinition)
        {
            _moduleSymbol = moduleSymbol;
            _methodSymbol = methodSymbol;
            _parameterType = parameterType;

            _parameterDefinition = parameterDefinition;
        }

        public override TypeSymbol Type => _parameterType;
        public override int Ordinal => _parameterDefinition.Index;
    }
}