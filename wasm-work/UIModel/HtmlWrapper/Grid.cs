using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using WebAssembly;
using WebAssembly.Browser.DOM;

namespace UIModel.HtmlWrapper
{
    public class Grid : AvaloniaObject
    {
        private List<int> _data;

        public Grid()
        {
            _topBuffer = new Tube<HTMLDivElement>(15);
            _bottomBuffer = new Tube<HTMLDivElement>(15);
            _rows = new Tube<HTMLDivElement>(15);

            void Superseded(object sender, IEnumerable<HTMLDivElement> elements)
            {
                foreach (var htmlDivElement in elements)
                {
                    _viewPort.RemoveChild(htmlDivElement);
                    htmlDivElement.Dispose();
                }
            }

            _topBuffer.OnSuperseded += Superseded;
            _bottomBuffer.OnSuperseded += Superseded;
        }

        private int _currentRowPosition = 0;

        private int _totalRows = 0;

        private Tube<HTMLDivElement> _topBuffer;
        private Tube<HTMLDivElement> _bottomBuffer;
        private Tube<HTMLDivElement> _rows;

        private HTMLDivElement _rootElement;
        private HTMLDivElement _viewPort;

        private int _defaultItemHeight = 30;

        public HTMLDivElement Root => _rootElement;

        public void Init()
        {
            _data = Enumerable.Range(1, 300).ToList();
            _totalRows = _data.Count;
            _rootElement = D.Doc.CreateElement<HTMLDivElement>();
            _viewPort = D.Doc.CreateElement<HTMLDivElement>();
            
            _rootElement.AppendChild(_viewPort);

            _rootElement.SetStyleAttribute("height", "300px");
            _rootElement.SetStyleAttribute("overflow", "scroll");

            _rootElement.OnScroll += (sender, args) =>
            {
                if (_rootElement.ScrollTop + _rootElement.ClientHeight >= _rootElement.ScrollHeight)
                {
                    Scroll(_currentRowPosition + _bottomBuffer.Count - 5);
                    return;
                }
                
                // if (_rootElement.ScrollTop < 10)
                // {
                //     Scroll(_currentRowPosition - _topBuffer.Count + 5);
                //     
                //     _rootElement.ScrollTop += 5;
                //     return;
                // }
            };

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine(_rootElement.IsCorrupted);
            
            Scroll(0);
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

            //update viewport
            _viewPort.SetStyleAttribute("height",
                $"{_defaultItemHeight * (_rows.Count + _topBuffer.Count + _bottomBuffer.Count)}px");
        }

        private HTMLDivElement FromIndex(int index)
        {
            var row = D.Doc.CreateElement<HTMLDivElement>();
            row.InnerText = _data[index].ToString();
            
            row.SetStyleAttribute("height", $"{_defaultItemHeight}px");

            _viewPort.AppendChild(row);

            return row;
        }
    }
}