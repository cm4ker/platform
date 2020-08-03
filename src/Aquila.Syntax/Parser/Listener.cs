using System;
using Antlr4.Runtime;

namespace Aquila.Syntax.Parser
{
    public class Listener : BaseErrorListener
    {
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine, string msg,
            RecognitionException e)
        {
            Console.WriteLine($"Error at line {line} at char {charPositionInLine}: {msg}");

            base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
        }
    }
}