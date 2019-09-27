using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using TextCopy;
using tterm.Ansi;
using tterm.Terminal;
using ZenPlatform.Shell.Ansi;
using ZenPlatform.SSH;

namespace ZenPlatform.Shell.Terminal
{
    /*
     * Терминал должен предоставлять следующие возможности
     * 1) Средства для ввода вывода
     * 2) Запуск какого-то интерфейса поверх себя
     * 3) Передача этому интерфейсу всех событий и комманды извне
     *
     *
     * SSH Client -> SSH Server -> Open Session -> Open new TerminalSession -> TerminalApplication
     *
     * Терминал предоставляет буфер для работы с текущей областью.
     * Приложение знает как рисовать себя в этой области, нужно ли двигать окно буфера и так далее
     *
     * SetCursor();
     * GetCursor();
     * OpenApplication(app)
     * CloseApplication();
     * 
     */

    internal class VirtualTerminal : ITerminal
    {
        private ExtendedStack<ITerminalApplication> _apps;

        private const int MaxHistorySize = 1024;

        private TerminalBufferChar[] _buffer;

        private TerminalSize _size;
        private readonly List<TerminalBufferLine> _history = new List<TerminalBufferLine>();

        /// <summary>
        /// Текущая позиция по оси Х
        /// </summary>
        private int _cursorX;

        /// <summary>
        /// Ткущая позиция по оси Y
        /// </summary>
        private int _cursorY;


//        public bool ShowCursor { get; set; }

        public void SetCursor(int x, int y)
        {
            _cursorX = x;
            _cursorY = y;
        }


        public CharAttributes CurrentCharAttributes { get; set; }

        public TerminalSelection Selection { get; set; }


        public VirtualTerminal(TerminalSize size)
        {
            _apps = new ExtendedStack<ITerminalApplication>();
            _buffer = new TerminalBufferChar[size.HeightRows * size.WidthColumns];

            _size = size;
            _cursorX = 0;
            _cursorY = 0;
        }

        private void Initialise(TerminalSize size)
        {
        }

        public event EventHandler<byte[]> OnData;

        private void PushData(byte[] data)
        {
            OnData?.Invoke(this, data);
        }


        private void PushData(string text)
        {
            PushData(Encoding.UTF8.GetBytes(text));
        }

        /// <summary>
        /// Индекс начиначется с 0
        /// </summary>
        public int CursorX => _cursorX;


        /// <summary>
        /// Индекс начиначется с 1
        /// </summary>
        public int CursorY => _cursorY;


        public void Consume(TerminalCode code)
        {
            switch (code.Type)
            {
                case TerminalCodeType.DeviceStatusResponce:
                    SetCursor(code.Column, code.Line);
                    break;
            }
        }

        public TerminalSize Size
        {
            get => _size;
            set
            {
                if (value != _size)
                {
                    var srcSize = _size;
                    var dstSize = value;

                    var srcBuffer = _buffer;
                    var dstBuffer = new TerminalBufferChar[dstSize.HeightRows * dstSize.WidthColumns];

                    int srcLeft = 0;
                    int srcRight = Math.Min((int) srcSize.WidthColumns, (int) dstSize.WidthColumns) - 1;
                    int srcTop = Math.Max(0, _cursorY - (int) dstSize.HeightRows + 1);
                    int srcBottom = srcTop + Math.Min((int) srcSize.HeightRows, (int) dstSize.HeightRows) - 1;
                    int dstLeft = 0;
                    int dstTop = 0;

                    CopyBufferToBuffer(srcBuffer, srcSize, srcLeft, srcTop, srcRight, srcBottom,
                        dstBuffer, dstSize, dstLeft, dstTop);

                    _buffer = dstBuffer;
                    _size = dstSize;

                    _cursorY = Math.Min(_cursorY, (int) _size.HeightRows - 1);
                }
            }
        }

        public void Clear()
        {
            ClearBlock(0, 0, (int) _size.WidthColumns - 1, (int) _size.HeightRows - 1);
        }

        public void ClearBlock(int left, int top, int right, int bottom)
        {
            left = Math.Max(0, left);
            top = Math.Max(0, top);
            right = Math.Min(right, (int) _size.WidthColumns - 1);
            bottom = Math.Min(bottom, (int) _size.HeightRows - 1);

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    int index = GetBufferIndex(x, y);
                    _buffer[index] = new TerminalBufferChar(' ', default(CharAttributes));
                }
            }

            //           ScrollToCursor();
        }

        internal void Type(char c)
        {
            if (IsInBuffer(_cursorX, _cursorY))
            {
                int index = GetBufferIndex(_cursorX, _cursorY);
                _buffer[index] = new TerminalBufferChar(c, CurrentCharAttributes);
                _cursorX++;
            }
        }

        internal void Type(string text)
        {
            foreach (char c in text)
            {
                if (IsInBuffer(_cursorX, _cursorY))
                {
                    int index = GetBufferIndex(_cursorX, _cursorY);
                    _buffer[index] = new TerminalBufferChar(c, CurrentCharAttributes);

                    var textForPush = GetText(_cursorX, _cursorY, (int) _size.WidthColumns - _cursorX);
                    PushData(textForPush);

                    _cursorX++;
                    Consume(new TerminalCode(TerminalCodeType.CursorPosition, _cursorY + 1, _cursorX + 1));
                }
            }

//            ScrollToCursor();
        }

        public void ShiftUp()
        {
            var topLine = GetBufferLine(0);
            AddHistory(topLine);

            for (int y = 0; y < _size.HeightRows - 1; y++)
            {
                Array.Copy(_buffer, (y + 1) * _size.WidthColumns, _buffer, y * _size.WidthColumns, _size.WidthColumns);
            }
        }

        public string GetText(int y)
        {
            return GetText(0, y, (int) _size.WidthColumns);
        }

        public string[] GetText(int left, int top, int right, int bottom)
        {
            int width = right - left + 1;
            int height = bottom - top + 1;
            string[] result = new string[height];
            for (int i = 0; i < height; i++)
            {
                result[i] = GetText(left, top + i, width);
            }

            return result;
        }

        public string GetText(int x, int y, int length)
        {
            if (x + length > _size.WidthColumns)
            {
                throw new ArgumentException("Range overflows line length.", nameof(length));
            }

            (var buffer, int startIndex, int endIndex) = GetLineBufferRange(y);
            if (buffer == null)
            {
                return string.Empty;
            }

            int maxLength = endIndex - startIndex;
            string line = GetTextAtIndex(buffer, startIndex + x, Math.Min(length, maxLength));
            return line;
        }

        private static string GetTextAtIndex(IList<TerminalBufferChar> buffer, int index, int length)
        {
            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = buffer[index + i].Char;
            }

            string line = new string(chars);
            return line;
        }

        public string[] GetSelectionText()
        {
            var selection = Selection;
            if (selection.Mode == SelectionMode.Stream)
            {
                (var start, var end) = selection.GetMinMax();
                var result = new List<string>();
                for (int y = start.Row; y <= end.Row; y++)
                {
                    int x0 = 0;
                    int x1 = (int) _size.WidthColumns - 1;
                    if (y == start.Row)
                    {
                        x0 = start.Column;
                    }

                    if (y == end.Row)
                    {
                        x1 = end.Column;
                    }

                    result.Add(GetText(x0, y, x1 - x0 + 1));
                }

                return result.ToArray();
            }
            else if (selection.Mode == SelectionMode.Block)
            {
                int left = Math.Min(selection.Start.Column, selection.End.Column);
                int right = Math.Max(selection.Start.Column, selection.End.Column);
                int top = Math.Min(selection.Start.Row, selection.End.Row);
                int bottom = Math.Max(selection.Start.Row, selection.End.Row);
                return GetText(left, top, right, bottom);
            }
            else
            {
                throw new InvalidOperationException("Unknown selection mode.");
            }
        }

        public void CopySelection()
        {
            string[] selectionText = GetSelectionText()
                .Select(x => x.TrimEnd())
                .ToArray();
            string text = String.Join(Environment.NewLine, selectionText);
            Clipboard.SetText(text);
        }

        private TerminalBufferLine GetBufferLine(int y)
        {
            int startIndex = GetBufferIndex(0, y);
            var bufferLine = new TerminalBufferLine(_buffer, startIndex, (int) _size.WidthColumns);
            return bufferLine;
        }

        public TerminalTagArray GetFormattedLine(int y)
        {
            var tags = ImmutableArray.CreateBuilder<TerminalTag>(initialCapacity: (int) _size.WidthColumns);

            (var buffer, int startIndex, int endIndex) = GetLineBufferRange(y);
            if (buffer == null)
            {
                AddFillerTags(tags, 0, y, (int) _size.WidthColumns);
                return new TerminalTagArray(tags.ToImmutable());
            }

            // Group sequentially by attribute
            int lineLength = endIndex - startIndex;
            var currentTagStartIndex = startIndex;
            var currentTagAttribute = GetAttributesAt(buffer[startIndex], 0, y);
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                int x = i - startIndex;
                var c = buffer[i];
                var attr = GetAttributesAt(c, x, y);
                if (!CanContinueTag(currentTagAttribute, attr, c.Char))
                {
                    string tagText = GetTextAtIndex(buffer, currentTagStartIndex, i - currentTagStartIndex);
                    tags.Add(new TerminalTag(tagText, currentTagAttribute));

                    currentTagStartIndex = i;
                    currentTagAttribute = attr;
                }
            }

            // Last tag
            {
                string tagText = GetTextAtIndex(buffer, currentTagStartIndex, endIndex - currentTagStartIndex);
                tags.Add(new TerminalTag(tagText, currentTagAttribute));
            }

            // Filler tags
            if (lineLength < _size.WidthColumns)
            {
                AddFillerTags(tags, lineLength, y, (int) _size.WidthColumns - lineLength);
            }

            return new TerminalTagArray(tags.ToImmutable());
        }

        public ExtendedStack<ITerminalApplication> Apps => _apps;

        public ITerminalApplication CurrentActive => (Apps.TryPeek(out var app)) ? app : null;

        public void Close(ITerminalApplication app)
        {
            lock (_apps)
            {
                if (CurrentActive == app)
                {
                    app.Data -= OnAppData;
                    _apps.Remove(app);
                }
            }
        }

        private void OnAppData(object sender, object e)
        {
            if (e is byte[] b)
                PushData(b);

            if (e is TerminalCode tc)
                Consume(tc);
        }

        public void Open(ITerminalApplication app)
        {
            lock (_apps)
            {
                if (CurrentActive == app) return;

                if (CurrentActive != null)
                {
                    CurrentActive.Data -= OnAppData;
                    CurrentActive.SetState(TerminalApplicationState.NotActive);
                }


                Apps.Push(app);
                app.Data += OnAppData;
                app.SetState(TerminalApplicationState.Run);
            }
        }

        private void AddFillerTags(ImmutableArray<TerminalTag>.Builder builder, int x, int y, int length)
        {
            int tagLength = 0;
            var lastAttr = GetAttributesAt(default(TerminalBufferChar), x, y);
            for (int i = 0; i < length; i++)
            {
                tagLength++;
                var attr = GetAttributesAt(default(TerminalBufferChar), x + i, y);
                if (attr != lastAttr)
                {
                    string text = new string(' ', tagLength);
                    builder.Add(new TerminalTag(text, lastAttr));
                    lastAttr = attr;
                    tagLength = 0;
                }
            }

            // Last tag
            {
                string text = new string(' ', tagLength);
                builder.Add(new TerminalTag(text, lastAttr));
            }
        }

        private CharAttributes GetAttributesAt(TerminalBufferChar bufferChar, int x, int y)
        {
            CharAttributes attr = bufferChar.Attributes;
            //TODO: Сделать текст разноцветным

//            if (ShowCursor && x == CursorX && y == CursorY)
//            {
//                attr.BackgroundColour = SpecialColourIds.Cursor;
//            }
//            else if (IsPointInSelection(x, y))
//            {
//                attr.BackgroundColour = SpecialColourIds.Selection;
//            }
//            else if (y < 0)
//            {
//                attr.BackgroundColour = SpecialColourIds.Historic;
//            }
//            else if (y >= _size.Rows)
//            {
//                attr.BackgroundColour = SpecialColourIds.Futuristic;
//            }

            return attr;
        }

        private (IList<TerminalBufferChar> buffer, int startIndex, int endIndex) GetLineBufferRange(int y)
        {
            if (y < 0)
            {
                int historyIndex = _history.Count + y;
                if (historyIndex < 0)
                {
                    return (null, 0, 0);
                }

                var historyLine = _history[historyIndex];
                var buffer = historyLine.Buffer;
                return (buffer, 0, (int) Math.Min(buffer.Length, _size.WidthColumns));
            }
            else
            {
                if (y >= _size.HeightRows)
                {
                    return (null, 0, 0);
                }

                int startIndex = GetBufferIndex(0, y);
                return (_buffer, startIndex, startIndex + (int) _size.WidthColumns);
            }
        }

        private bool IsPointInSelection(int x, int y)
        {
            var selection = Selection;
            if (selection == null)
            {
                return false;
            }
            else if (selection.Mode == SelectionMode.Stream)
            {
                (var start, var end) = selection.GetMinMax();
                if (y == start.Row)
                {
                    if (x < start.Column)
                    {
                        return false;
                    }
                }

                if (y == end.Row)
                {
                    if (x > end.Column)
                    {
                        return false;
                    }
                }

                if (y < start.Row || y > end.Row)
                {
                    return false;
                }

                return true;
            }
            else if (selection.Mode == SelectionMode.Block)
            {
                int left = Math.Min(selection.Start.Column, selection.End.Column);
                int right = Math.Max(selection.Start.Column, selection.End.Column);
                int top = Math.Min(selection.Start.Row, selection.End.Row);
                int bottom = Math.Max(selection.Start.Row, selection.End.Row);
                return (x >= left && x <= right &&
                        y >= top && y <= bottom);
            }
            else
            {
                throw new InvalidOperationException("Unknown selection mode.");
            }
        }

        private static bool CanContinueTag(CharAttributes previous, CharAttributes next, char nextC)
        {
            if (nextC == ' ')
            {
                return previous.BackgroundColour == next.BackgroundColour;
            }
            else
            {
                return previous == next;
            }
        }

        public bool IsInBuffer(int x, int y)
        {
            return (x >= 0 && x < _size.WidthColumns && y >= 0 && y < _size.HeightRows);
        }

        private int GetBufferIndex(int x, int y)
        {
            return x + (y * (int) _size.WidthColumns);
        }

        private TerminalPoint GetBufferPoint(int index)
        {
            return new TerminalPoint(index % (int) _size.WidthColumns, index % (int) _size.WidthColumns);
        }

        private void AddHistory(TerminalBufferLine line)
        {
            if (_history.Count >= MaxHistorySize)
            {
                _history.RemoveAt(0);
            }

            _history.Add(line);
        }

//        public void Scroll(int scroll)
//        {
//            int top = WindowTop + scroll;
//            top = Math.Max(top, -_history.Count);
//            // top = Math.Min(top, _size.Rows - 1);
//            top = Math.Min(top, CursorY);
//            WindowTop = top;
//        }
//
//        public void ScrollToCursor()
//        {
//            int windowTop = WindowTop;
//            int windowBottom = WindowBottom;
//            if (CursorY < windowTop)
//            {
//                WindowTop = CursorY;
//            }
//            else if (CursorY > windowBottom)
//            {
//                WindowBottom = CursorY;
//            }
//        }

        private static void CopyBufferToBuffer(TerminalBufferChar[] srcBuffer, TerminalSize srcSize, int srcLeft,
            int srcTop, int srcRight, int srcBottom,
            TerminalBufferChar[] dstBuffer, TerminalSize dstSize, int dstLeft, int dstTop)
        {
            int cols = srcRight - srcLeft + 1;
            int rows = srcBottom - srcTop + 1;
            for (int y = 0; y < rows; y++)
            {
                int srcY = srcTop + y;
                int dstY = dstTop + y;
                int srcIndex = srcLeft + (srcY * (int) srcSize.WidthColumns);
                int dstIndex = dstLeft + (dstY * (int) dstSize.WidthColumns);
                Array.Copy(srcBuffer, srcIndex, dstBuffer, dstIndex, cols);
            }
        }

        public void MoveCursorForward()
        {
            _cursorX++;
        }

        public void MoveCursorBack()
        {
            _cursorX--;
        }
    }
}