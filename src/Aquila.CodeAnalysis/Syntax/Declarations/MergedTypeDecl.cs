using System.Collections.Generic;
using Aquila.CodeAnalysis.Syntax;

namespace Aquila.Syntax.Declarations;

public class MergedTypeDecl
{
    public MergedTypeDecl(TypeDecl type, IEnumerable<FuncDecl> funcs)
    {
        TypeDecl = type;
        FuncDecls = funcs;
    }

    public string Name => TypeDecl.Name.GetUnqualifiedName().Identifier.Text;

    public TypeDecl TypeDecl { get; }
    public IEnumerable<FuncDecl> FuncDecls { get; }
}