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
    }
}