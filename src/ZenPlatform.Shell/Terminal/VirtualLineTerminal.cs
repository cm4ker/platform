using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ZenPlatform.Shell.Ansi;
using ZenPlatform.Shell.Contracts.Ansi;
using ZenPlatform.SSH;

namespace ZenPlatform.Shell.Terminal
{
    /// <summary>
    /// Управляет единичной командой в терминале
    /// </summary>
    internal class VirtualLineTerminal
    {
        private IList<TerminalBufferChar> _currentChars;

        private int _cursorPosition = 0;

        /// <summary>
        /// Где начинается линия
        /// </summary>
        private int _startPositionX, _startPositionY;

        public VirtualLineTerminal(int startPositionX, int startPositionY, TerminalBufferChar buffer)
        {
            _startPositionX = startPositionX;
            _startPositionY = startPositionY;

            _currentChars = new List<TerminalBufferChar>();
        }

        internal void UpdateStartPosition(int x, int y)
        {
            _startPositionX = x;
            _startPositionY = y;
        }

        public event EventHandler OnUpdate;

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