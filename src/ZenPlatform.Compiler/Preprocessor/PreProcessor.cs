using System;
using System.Collections.Generic;
using Antlr4.Runtime;

namespace ZenPlatform.Compiler.Preprocessor
{
    /// <summary>
    /// Препроцессор языка.
    /// Позволяет обрабатывать сначала код, а потом уже семантически разбирать его
    /// </summary>
    public static class PreProcessor
    {
        public static ITokenStream Do(AntlrInputStream stream, List<string> conditionalSymbols = null)
        {
            List<IToken> codeTokens = new List<IToken>();
            List<IToken> commentTokens = new List<IToken>();

            Lexer preprocessorLexer = new ZSharpLexer(stream);

            var tokens = preprocessorLexer.GetAllTokens();
            var directiveTokens = new List<IToken>();
            var directiveTokenSource = new ListTokenSource(directiveTokens);
            var directiveTokenStream = new CommonTokenStream(directiveTokenSource, ZSharpLexer.DIRECTIVE);
            ZSharpPreprocessorParser preprocessorParser = new ZSharpPreprocessorParser(directiveTokenStream);

            preprocessorParser.ConditionalSymbols.Clear();

            conditionalSymbols?.ForEach(x => preprocessorParser.ConditionalSymbols.Add(x));

            int index = 0;
            bool compiliedTokens = true;
            while (index < tokens.Count)
            {
                var token = tokens[index];
                if (token.Type == ZSharpLexer.SHARP)
                {
                    directiveTokens.Clear();
                    int directiveTokenIndex = index + 1;

                    while (directiveTokenIndex < tokens.Count &&
                           tokens[directiveTokenIndex].Type != ZSharpLexer.Eof &&
                           tokens[directiveTokenIndex].Type != ZSharpLexer.DIRECTIVE_NEW_LINE &&
                           tokens[directiveTokenIndex].Type != ZSharpLexer.SHARP)
                    {
                        if (tokens[directiveTokenIndex].Channel == ZSharpLexer.COMMENTS_CHANNEL)
                        {
                            commentTokens.Add(tokens[directiveTokenIndex]);
                        }
                        else if (tokens[directiveTokenIndex].Channel != Lexer.Hidden)
                        {
                            directiveTokens.Add(tokens[directiveTokenIndex]);
                        }

                        directiveTokenIndex++;
                    }

                    directiveTokenSource = new ListTokenSource(directiveTokens);
                    directiveTokenStream = new CommonTokenStream(directiveTokenSource, ZSharpLexer.DIRECTIVE);
                    preprocessorParser.SetInputStream(directiveTokenStream);
                    preprocessorParser.Reset();

                    // Parse condition in preprocessor directive (based on CSharpPreprocessorParser.g4 grammar).
                    ZSharpPreprocessorParser.Preprocessor_directiveContext directive =
                        preprocessorParser.preprocessor_directive();
                    // if true than next code is valid and not ignored.
                    compiliedTokens = directive.value;
                    string directiveStr = tokens[index + 1].Text.Trim();
                    if ("line".Equals(directiveStr) || "error".Equals(directiveStr) || "warning".Equals(directiveStr) ||
                        "define".Equals(directiveStr) || "endregion".Equals(directiveStr) ||
                        "endif".Equals(directiveStr) || "pragma".Equals(directiveStr))
                    {
                        compiliedTokens = true;
                    }

                    string conditionalSymbol = null;
                    if ("define".Equals(tokens[index + 1].Text))
                    {
                        // add to the conditional symbols 
                        conditionalSymbol = tokens[index + 2].Text;
                        preprocessorParser.ConditionalSymbols.Add(conditionalSymbol);
                    }

                    if ("undef".Equals(tokens[index + 1].Text))
                    {
                        conditionalSymbol = tokens[index + 2].Text;
                        preprocessorParser.ConditionalSymbols.Remove(conditionalSymbol);
                    }

                    index = directiveTokenIndex - 1;


                    compiliedTokens = directive.value;
                    index = directiveTokenIndex - 1;
                }
//                else if (token.Channel == CommentsChannel)
//                {
//                    commentTokens.Add(token); // Colect comment tokens (if required).
//                }
                else if (token.Channel != Lexer.Hidden && token.Type != ZSharpLexer.DIRECTIVE_NEW_LINE &&
                         compiliedTokens)
                {
                    codeTokens.Add(token);
                }

                index++;
            }

            var codeTokenSource = new ListTokenSource(codeTokens);

            var codeTokenStream = new CommonTokenStream(codeTokenSource);
            return codeTokenStream;
        }
    }
}