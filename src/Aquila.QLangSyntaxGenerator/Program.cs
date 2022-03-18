using System;
using System.Text;
using Aquila.SyntaxGenerator.QLang;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.QLangSyntaxGenerator
{
    [Generator]
    public class AstSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var unit = QLangSyntaxGeneratorHelper.Generate(context);
            context.AddSource("QLang.models.generated.cs",
                SourceText.From(unit.NormalizeWhitespace().ToFullString(), Encoding.UTF8));

            context.AddSource("QLang.rewriter.generated.cs",
                SourceText.From(QLangSyntaxGeneratorHelper.GenerateRewriter(context), Encoding.UTF8));
        }
    }
}