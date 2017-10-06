using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;

namespace SqlPlusDbSync.Platform.Syntax
{
    public class ErrorListener : BaseErrorListener
    {
        public ErrorListener()
        {
        }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
            throw new Exception("Syntax error");
        }
    }
}
