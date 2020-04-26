using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Data;
using UIModel;
using UIModel.HtmlWrapper;
using UIModel.XML;

namespace Tester
{
    public class Row
    {
        public string Content { get; set; }
    }

    public class Grid : AvaloniaObject
    {
        private List<int> _data;

        public Grid()
        {
            _topBuffer = new Tube<Row>(15);
            _bottomBuffer = new Tube<Row>(15);
            _rows = new Tube<Row>(15);
        }

        private int _currentRowPosition = 0;

        private int _totalRows = 0;

        private Tube<Row> _topBuffer;
        private Tube<Row> _bottomBuffer;
        private Tube<Row> _rows;

        public void Init()
        {
            _data = Enumerable.Range(1, 300).ToList();
            _totalRows = _data.Count;
            Scroll(0);
        }

        private void CalculateViewPort()
        {
        }

        int GetViewPortSize()
        {
            return Math.Min(Math.Min(_rows.MaxItems, _topBuffer.MaxItems), _bottomBuffer.MaxItems);
        }

        public void Scroll(int rowPosition)
        {
            rowPosition = Math.Min(rowPosition, _totalRows);

            var diff = rowPosition - _currentRowPosition;

            //if we scroll and this scroll more than buffered zone
            if (diff > GetViewPortSize() || _rows.Count == 0)
            {
                _topBuffer.Clear();
                _bottomBuffer.Clear();
                _rows.Clear();

                if (rowPosition > 0)
                {
                    var index = rowPosition - 1;
                    var counter = _topBuffer.MaxItems;
                    while (index > 0 && counter > 0)
                    {
                        _topBuffer.PushTop(FromIndex(index));
                        index--;
                        counter--;
                    }
                }

                // create viewPort
                {
                    var index = rowPosition;
                    var counter = _rows.MaxItems;

                    while (index < _totalRows && counter > 0)
                    {
                        _rows.PushBottom(FromIndex(index));
                        index++;
                        counter--;
                    }

                    counter = _bottomBuffer.MaxItems;

                    //create bottom buffer
                    while (index < _totalRows && counter > 0)
                    {
                        _bottomBuffer.PushBottom(FromIndex(index));
                        index++;
                        counter--;
                    }
                }
            }
            else
            {
                if (diff > 0 && _bottomBuffer.Count > 0)
                {
                    diff = Math.Min(diff, _bottomBuffer.Count);

                    _topBuffer.PushBottom(_rows.TakeTopRange(diff));
                    _rows.PushBottom(_bottomBuffer.TakeTopRange(diff));

                    var lastItem = rowPosition + _rows.MaxItems + _bottomBuffer.MaxItems - diff;
                    var diffCounter = diff;

                    while (_totalRows > lastItem && diffCounter > 0)
                    {
                        _bottomBuffer.PushBottom(FromIndex(lastItem));

                        lastItem++;
                        diffCounter--;
                    }
                }

                else if (diff < 0 && _topBuffer.Count > 0)
                {
                    diff = Math.Min(-diff, _topBuffer.Count);

                    _bottomBuffer.PushTop(_rows.TakeBottomRange(diff));
                    _rows.PushTop(_topBuffer.TakeBottomRange(diff));

                    var firstTopItem = rowPosition - _topBuffer.Count - 1;
                    var diffCounter = diff;

                    while (0 <= firstTopItem && diffCounter > 0)
                    {
                        _topBuffer.PushTop(FromIndex(firstTopItem));

                        firstTopItem--;
                        diffCounter--;
                    }
                }
            }

            _currentRowPosition = rowPosition;
        }

        private Row FromIndex(int index)
        {
            var row = new Row();
            row.Content = _data[index].ToString();

            return row;
        }

        private IEnumerable<Row> AllocatedRows()
        {
            foreach (var row in _topBuffer)
            {
                yield return row;
            }

            foreach (var row in _rows)
            {
                yield return row;
            }

            foreach (var row in _bottomBuffer)
            {
                yield return row;
            }
        }

        public void Draw()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (var row in _topBuffer)
            {
                Console.WriteLine(row.Content);
            }

            Console.ForegroundColor = ConsoleColor.White;
            foreach (var row in _rows)
            {
                Console.WriteLine(row.Content);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var row in _bottomBuffer)
            {
                Console.WriteLine(row.Content);
            }
        }
    }

    class Program
    {
        static void Main2(string[] args)
        {
            Console.Clear();

            var g = new Grid();

            g.Init();
            var pos = 0;
            g.Draw();

            var key = Console.ReadKey();

            while (key != new ConsoleKeyInfo('r', ConsoleKey.R, false, false, false))
            {
                if (key.Key == ConsoleKey.DownArrow)
                {
                    pos++;
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    if (pos > 0)
                        pos--;
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    pos += 40;
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (pos > 40)
                        pos -= 40;
                }

                Console.Clear();
                g.Scroll(pos);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                g.Draw();
                sw.Stop();

                Console.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");

                key = Console.ReadKey();
            }
        }

        public class AnyRef
        {
            ~AnyRef()
            {
                Console.WriteLine($"The {ToString()} destructor is executing.");
            }
        }

        public static void Main(string[] args)
        {
            var a = new AnyRef();
    
            var hand = GCHandle.Alloc(a, GCHandleType.Normal);


            Console.WriteLine(hand.IsAllocated);

            hand.Free();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            while (true)
            {
            }

            //Console.WriteLine(a.Handle.IsAllocated);
        }
    }
}