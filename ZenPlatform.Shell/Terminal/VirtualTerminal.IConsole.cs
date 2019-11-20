using System;
using System.IO;
using ZenPlatform.Shell.Ansi;

namespace ZenPlatform.Shell.Terminal
{
    public partial class VirtualTerminal 
    {
        

  
        /// <summary>
        /// Индекс начиначется с 0
        /// </summary>
        public int CursorLeft => _cursorX;

        /// <summary>
        /// Индекс начиначется с 1
        /// </summary>
        public int CursorTop => _cursorY;

    }
}