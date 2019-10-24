using System.Collections;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Atn;
using ServiceStack;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    /// <summary>
    /// Элемент логических связей в запросе. LT - Logical tree
    /// </summary>
    public abstract class LTItem
    {
        public string Token;
    }
}