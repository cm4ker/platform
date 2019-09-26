using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TextCopy;
using tterm.Ansi;
using tterm.Terminal;

namespace ZenPlatform.Shell.Terminal
{
    internal enum InsertionMode
    {
        Replace,
        Insert
    }

    internal class TerminalCommandBuffer
    {
        private IList<TerminalBufferChar> _currentChars;

        private int _cursorPosition = 0;

        public TerminalCommandBuffer()
        {
            _currentChars = new List<TerminalBufferChar>();
        }

        public void MoveCursorForward()
        {
            if (_currentChars.Count > _cursorPosition)
                _cursorPosition++;
        }

        public void MoveCursorBack()
        {
            if (_cursorPosition > 0)
                _cursorPosition--;
        }

        public string Flush()
        {
            var result = new string(_currentChars.Select(x => x.Char).ToArray());
            Clear();
            return result;
        }

        public void Type(char c)
        {
            var termChar = new TerminalBufferChar(c, new CharAttributes());

            if (_cursorPosition == _currentChars.Count)
            {
                _currentChars.Add(termChar);
               
            }
            else
            {
                _currentChars.Insert(_cursorPosition, termChar);
            }

            MoveCursorForward();
        }

        static string ToArrayString(IEnumerable<char> charSequence)
        {
            return new String(charSequence.ToArray());
        }

        public string GetText()
        {
            return ToArrayString(_currentChars.Skip(_cursorPosition - 1).Select(x => x.Char));
        }

        public void Clear()
        {
            _currentChars.Clear();
            _cursorPosition = 0;
        }

        public void Type(string str)
        {
            foreach (var c in str)
            {
                Type(c);
            }
        }

        public void Backspace()
        {
            _currentChars.RemoveAt(_cursorPosition);
            MoveCursorBack();
        }
    }
}