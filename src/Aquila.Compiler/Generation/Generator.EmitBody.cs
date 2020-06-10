using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Language.Ast.Definitions;
using Statement = Aquila.Language.Ast.Definitions.Statements.Statement;

namespace Aquila.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitBody(PCilBody e, Block body, PLabel returnLabel, ref PLocal resultvar, bool inTry = false)
        {
            foreach (Statement statement in body.Statements)
            {
                EmitStatement(e, statement, body, returnLabel, ref resultvar, inTry);
            }
        }
    }
}