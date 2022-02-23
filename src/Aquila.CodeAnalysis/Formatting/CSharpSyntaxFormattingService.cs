using System.Text;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;

namespace Aquila.Formatting;

public class PrettyPrinter
{
    public static string Print(AquilaSyntaxTree tree)
    {
        return " ";
    }
}

public class DocumentPrinterVisitor : AquilaSyntaxVisitor
{
    private StringBuilder _builder = new();

    public override void VisitIdentifierEx(IdentifierEx node)
    {
        _builder.Append(node.Identifier);
    }
}