using ZenPlatform.Shell.Ansi;

namespace ZenPlatform.Shell.Terminal
{
    public struct TerminalBufferChar
    {
        public char Char { get; }
        public CharAttributes Attributes { get; }

        public TerminalBufferChar(char c, CharAttributes attr)
        {
            Char = c;
            Attributes = attr;
        }

        public override string ToString()
        {
            return Char.ToString();
        }
    }
}
