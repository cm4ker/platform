using System;
using tterm.Ansi;
using tterm.Terminal;
using ZenPlatform.SSH;

namespace ZenPlatform.Shell.Terminal
{
    internal static class Helpers
    {
        public static void CopyBufferToBuffer(this TerminalBufferChar[] srcBuffer, TerminalSize srcSize, int srcLeft,
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
    }

    internal partial class VirtualTerminal
    {
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

        public bool IsInBuffer(int x, int y)
        {
            return (x >= 0 && x < _size.WidthColumns && y >= 0 && y < _size.HeightRows);
        }
    }
}