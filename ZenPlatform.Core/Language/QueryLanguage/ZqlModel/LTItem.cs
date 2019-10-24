﻿using System.Text;
using Antlr4.Runtime.Atn;

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