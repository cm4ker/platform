namespace Aquila.Syntax.Ast
{
    partial record TypeRef
    {
        public bool IsVar => this.Kind == SyntaxKind.VarKeyword;
    }
}