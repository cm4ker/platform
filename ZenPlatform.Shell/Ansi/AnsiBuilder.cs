using System;
using System.Text;

namespace ZenPlatform.Shell.Ansi
{
    internal class AnsiBuilder
    {
        private const char Bracket = '[';
        private const char D = 'D';
        private const char H = 'H';
        private const char C = 'C';
        private const char Six = '6';
        private const char n = 'n';
        private const char Semicolon = ';';

        public static byte[] Build(TerminalCode code)
        {
            return code.Type switch
            {
                TerminalCodeType.CursorBackward => Conv(C0.ESC, Bracket, D),
                TerminalCodeType.CursorForward => Conv(C0.ESC, Bracket, C),
                TerminalCodeType.Backspace => Conv(C0.BS),
                TerminalCodeType.CursorPosition => Conv(
                    "" + C0.ESC + Bracket + code.Line + Semicolon + code.Column + H),
                TerminalCodeType.Text => Conv(code.Text),
                TerminalCodeType.DeviceStatusRequest => Conv(C0.ESC, Bracket, Six, n),
                TerminalCodeType.LineFeed => Conv(C0.CR, C0.LF),
                _ => throw new Exception()
            };
        }


        private static byte[] Conv(params char[] data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        private static byte[] Conv(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public static byte[] SetCursorPosCommand(int cursorLine, int cursorColumn)
        {
            var res = "" + C0.ESC + Bracket + cursorLine + Semicolon + cursorColumn + H;
            return Encoding.UTF8.GetBytes(res);
        }
    }
}