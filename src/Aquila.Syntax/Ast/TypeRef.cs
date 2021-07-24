namespace Aquila.Syntax.Ast
{
    partial class TypeRef
    {
        public bool IsVar => this.Kind == SyntaxKind.VarKeyword;
    }
}