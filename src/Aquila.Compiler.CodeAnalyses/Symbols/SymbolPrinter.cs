using System;
using System.IO;

namespace Aquila.Language.Ast.Symbols
{
    internal static class SymbolPrinter
    {
        public static void WriteTo(Symbol symbol, TextWriter writer)
        {
            switch (symbol.Kind)
            {
                // case SymbolKind.Method:
                //     WriteFunctionTo((MethodSymbol) symbol, writer);
                //     break;
                // case SymbolKind.Global:
                //     WriteGlobalVariableTo((GlobalLocalSymbol) symbol, writer);
                //     break;
                // case SymbolKind.Local:
                //     WriteLocalVariableTo((LocalLocalSymbol) symbol, writer);
                //     break;
                // case SymbolKind.Parameter:
                //     WriteParameterTo((ParameterSymbol) symbol, writer);
                //     break;
                // case SymbolKind.NamedType:
                //     WriteTypeTo((NamedTypeSymbol) symbol, writer);
                //     break;
                default:
                    throw new Exception($"Unexpected symbol: {symbol.Kind}");
            }
        }
        //
        // private static void WriteFunctionTo(MethodSymbol symbol, TextWriter writer)
        // {
        //     writer.WriteKeyword(SyntaxKind.FunctionKeyword);
        //     writer.WriteSpace();
        //     writer.WriteIdentifier(symbol.Name);
        //     writer.WritePunctuation(SyntaxKind.OpenParenthesisToken);
        //
        //     for (int i = 0; i < symbol.Parameters.Length; i++)
        //     {
        //         if (i > 0)
        //         {
        //             writer.WritePunctuation(SyntaxKind.CommaToken);
        //             writer.WriteSpace();
        //         }
        //
        //         symbol.Parameters[i].WriteTo(writer);
        //     }
        //
        //     writer.WritePunctuation(SyntaxKind.CloseParenthesisToken);
        //
        //     if (symbol.NamedType.SpecialType == SpecialType.System_Void)
        //     {
        //         writer.WritePunctuation(SyntaxKind.ColonToken);
        //         writer.WriteSpace();
        //         symbol.NamedType.WriteTo(writer);
        //     }
        // }
        //
        // private static void WriteGlobalVariableTo(GlobalLocalSymbol symbol, TextWriter writer)
        // {
        //     writer.WriteKeyword(symbol.IsReadOnly ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword);
        //     writer.WriteSpace();
        //     writer.WriteIdentifier(symbol.Name);
        //     writer.WritePunctuation(SyntaxKind.ColonToken);
        //     writer.WriteSpace();
        //     symbol.NamedType.WriteTo(writer);
        // }
        //
        // private static void WriteLocalVariableTo(LocalLocalSymbol symbol, TextWriter writer)
        // {
        //     writer.WriteKeyword(symbol.IsReadOnly ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword);
        //     writer.WriteSpace();
        //     writer.WriteIdentifier(symbol.Name);
        //     writer.WritePunctuation(SyntaxKind.ColonToken);
        //     writer.WriteSpace();
        //     symbol.NamedType.WriteTo(writer);
        // }
        //
        // private static void WriteParameterTo(ParameterSymbol symbol, TextWriter writer)
        // {
        //     writer.WriteIdentifier(symbol.Name);
        //     writer.WritePunctuation(SyntaxKind.ColonToken);
        //     writer.WriteSpace();
        //     symbol.Type.WriteTo(writer);
        // }
        //
        // private static void WriteTypeTo(NamedTypeSymbol symbol, TextWriter writer)
        // {
        //     writer.WriteIdentifier(symbol.Name);
        // }
    }
}