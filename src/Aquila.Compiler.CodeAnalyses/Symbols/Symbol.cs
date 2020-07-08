using System.IO;

namespace Aquila.Language.Ast.Symbols
{
    public abstract class Symbol
    {
        public abstract SymbolKind Kind { get; }
        public virtual string Name => string.Empty;
        public virtual Symbol ContainingSymbol => null;

        internal virtual ModuleSymbol? ContainingModule
        {
            get
            {
                var container = this.ContainingSymbol;
                return (object) container != null ? container.ContainingModule : null;
            }
        }

        public void WriteTo(TextWriter writer)
        {
            SymbolPrinter.WriteTo(this, writer);
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}