namespace Aquila.CodeAnalysis.Syntax
{
    public partial class NameEx
    {
        internal abstract SimpleNameEx GetUnqualifiedName();
    }

    public partial class SimpleNameEx
    {
        internal override SimpleNameEx GetUnqualifiedName()
        {
            return this;
        }
    }

    public partial class QualifiedNameEx
    {
        internal override SimpleNameEx GetUnqualifiedName()
        {
            return Right;
        }

        internal override string GetName()
        {
            return GetUnqualifiedName().Identifier.Text;
        }
    }

    public partial class TypeEx
    {
        public bool IsVar => ((InternalSyntax.TypeEx)this.Green).IsVar;

        public bool IsUnmanaged => ((InternalSyntax.TypeEx)this.Green).IsUnmanaged;

        public bool IsNotNull => ((InternalSyntax.TypeEx)this.Green).IsNotNull;

        public bool IsNint => ((InternalSyntax.TypeEx)this.Green).IsNint;

        public bool IsNuint => ((InternalSyntax.TypeEx)this.Green).IsNuint;

        internal virtual string GetName() => ToString();
    }
}