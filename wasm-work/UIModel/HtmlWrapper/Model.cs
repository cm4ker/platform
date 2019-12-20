using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.PlatformServices;
using System.Runtime.CompilerServices;
using Avalonia;
using UIModel.XML;
using WebAssembly.Browser.DOM;

namespace UIModel.HtmlWrapper
{
    internal static class D
    {
        public static Document Doc { get; } = new Document();
    }

    public class Button : AvaloniaObject
    {
    }

    public class Field : AvaloniaObject
    {
        public Field()
        {
        }

        public int Height { get; set; }

        public int Width { get; set; }

        public object Value { get; set; }
    }


    public class UIContainer : StyledElement
    {
    }

    public class ObjectPickerField : AvaloniaObject
    {
        private HTMLInputElement _htmlInput;
        private HTMLButtonElement _htmlClearButton;
        private HTMLButtonElement _htmlLookupButton;
        private HTMLButtonElement _htmlSelectTypeButton;
        private HTMLDivElement _htmlLayout;

        private bool _isPicker = false;

        int _currentFocus = -1;

        public HTMLElement Root => _htmlLayout;

        public ObjectPickerField()
        {
            _htmlInput = D.Doc.CreateElement<HTMLInputElement>();
            _htmlClearButton = D.Doc.CreateElement<HTMLButtonElement>();
            _htmlLookupButton = D.Doc.CreateElement<HTMLButtonElement>();
            _htmlSelectTypeButton = D.Doc.CreateElement<HTMLButtonElement>();

            _htmlLayout = D.Doc.CreateElement<HTMLDivElement>();
            _htmlLayout.AppendChild(_htmlInput);
            _htmlLayout.AppendChild(_htmlClearButton);
            _htmlLayout.AppendChild(_htmlLookupButton);
            _htmlLayout.AppendChild(_htmlSelectTypeButton);

            var a = D.Doc.GetElementById("tset");

            SetPicker(false);
        }

        private void SetPicker(bool isPicker)
        {
            _isPicker = isPicker;

            if (_isPicker)
            {
                // add autocomplete
            }
            else
            {
                // _htmlInput.OnKeyup += KeyUp;
                // _htmlInput.OnKeydown += KeyDown;
                // _htmlInput.OnKeypress += KeyPress;
                _htmlInput.OnInput += HtmlInputOnOnInput;
            }
        }

        private void HtmlInputOnOnInput(DOMObject sender, DOMEventArgs args)
        {
            Value = _htmlInput.Value;
        }

        private void KeyPress(DOMObject sender, DOMEventArgs args)
        {
            Value = _htmlInput.Value;
        }

        private void KeyDown(DOMObject sender, DOMEventArgs args)
        {
            Value = _htmlInput.Value;
        }

        private void KeyUp(DOMObject sender, DOMEventArgs args)
        {
            Value = _htmlInput.Value;
        }

        public void InitAutocomplete(List<string> arr)
        {
            void ElementOnOnInput(DOMObject sender, DOMEventArgs args)
            {
                var val = _htmlInput.Value;

                CloseAllLists(null);

                if (string.IsNullOrEmpty(val))
                {
                    return;
                }

                _currentFocus = -1;

                var a = D.Doc.CreateElement<HTMLDivElement>();

                a.SetAttribute("id", _htmlInput.Id + "autocomplete-list");
                a.SetAttribute("class", "autocomplete-items");

                _htmlLayout.AppendChild(a);

                for (int i = 0; i < arr.Count; i++)
                {
                    var itemLower = arr[i].ToLower();
                    var valLower = val.ToLower();

                    if (itemLower.Contains(valLower, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var b = D.Doc.CreateElement<HTMLDivElement>();

                        var index = itemLower.IndexOf(valLower, StringComparison.InvariantCultureIgnoreCase); //0
                        var currentPos = 0;

                        while (index >= 0)
                        {
                            if (index != currentPos && index > currentPos) // false, true
                            {
                                b.InnerHtml += arr[i].Substring(currentPos, index - currentPos);
                            }

                            b.InnerHtml += "<strong>" + arr[i].Substring(index, val.Length) + "</strong>"; // T

                            currentPos = index + val.Length; // currentPos = 1

                            index = itemLower.IndexOf(valLower, currentPos,
                                StringComparison.InvariantCultureIgnoreCase); // 3
                        }

                        if (currentPos < arr[i].Length)
                        {
                            b.InnerHtml += arr[i].Substring(currentPos);
                        }

                        b.InnerHtml += "<input type='hidden' value='" + arr[i] + "'>";

                        b.OnClick += (o, eventArgs) =>
                        {
                            Value = b.GetElementsByTagName("input")[0].ConvertTo<HTMLInputElement>()
                                .Value;
                            //TODO: set Value property

                            CloseAllLists(null);
                        };

                        a.AppendChild(b);
                    }
                }
            }

            void ElementOnOnKeydown(DOMObject sender, DOMEventArgs args)
            {
                var x = D.Doc.GetElementById(_htmlInput.Id + "autocomplete-list");

                if (x == null || x.IsCorrupted)
                {
                    return;
                }

                var elemList = x.GetElementsByTagName("div");

                if (elemList == null)
                {
                    return;
                }

                if (args.KeyCode == 40)
                {
                    _currentFocus++;
                    AddActive(elemList);
                }
                else if (args.KeyCode == 38)
                {
                    _currentFocus--;
                    AddActive(elemList);
                }
                else if (args.KeyCode == 13)
                {
                    args.PreventDefault();

                    if (_currentFocus > -1)
                    {
                        elemList[_currentFocus].ConvertTo<HTMLDivElement>().Click();
                    }
                }
            }

            _htmlInput.OnInput += ElementOnOnInput;
            _htmlInput.OnKeydown += ElementOnOnKeydown;

            _htmlLayout.ClassName += " autocomplete";
            //_htmlInput.OnFocusOut += (sender, args) => { CloseAllLists(null); };
        }

        private void AddActive(NodeListOf<Element> elements)
        {
            if (elements == null)
                return;

            RemoveActive(elements);

            if (_currentFocus >= elements.Length) _currentFocus = 0;
            if (_currentFocus < 0) _currentFocus = (int) elements.Length - 1;

            Console.WriteLine($"Current focus {_currentFocus}");
            Console.WriteLine($"Current focus {elements[_currentFocus]}");
            elements[_currentFocus]?.ClassList?.Add("autocomplete-active");
        }

        private void RemoveActive(NodeListOf<Element> elements)
        {
            if (elements != null)
                foreach (var element in elements)
                {
                    element.ClassList.Remove("autocomplete-active");
                }
        }

        private void CloseAllLists(HTMLElement except)
        {
            var x = D.Doc.GetElementsByClassName("autocomplete-items")?.ToArray();

            if (x != null)
                foreach (var element in x)
                {
                    if (element != except)
                    {
                        element.ParentNode.RemoveChild(element);
                    }
                }
        }

        /// <summary>
        /// Defines the <see cref="Text"/> property.
        /// </summary>
        public static readonly DirectProperty<ObjectPickerField, object> ValueProperty =
            AvaloniaProperty.RegisterDirect<ObjectPickerField, object>(
                nameof(Value),
                o => o.Value,
                (o, v) => o.Value = v);

        private object _value;

        public object Value
        {
            get { return _value; }
            set
            {
                SetAndRaise(ValueProperty, ref _value, value);
                _htmlInput.Value = value.ToString();
            }
        }
    }

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
                    htmlDivElement.ParentElement.RemoveChild(htmlDivElement);
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

        public HTMLDivElement _rootElement;
        public HTMLDivElement _viewPort;

        public HTMLDivElement Root => _rootElement;

        public void Init()
        {
            _data = Enumerable.Range(1, 300).ToList();
            _totalRows = _data.Count;
            _rootElement = D.Doc.CreateElement<HTMLDivElement>();
            _viewPort = D.Doc.CreateElement<HTMLDivElement>();
            _rootElement.AppendChild(_viewPort);
            
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
/*

 1
 2
 --top
 3 <---- current position (2 - pos)
 4
 --rows
 5
  --bottom
 6 < -- last item
 7
 
 */
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

        private HTMLDivElement FromIndex(int index)
        {
            var row = D.Doc.CreateElement<HTMLDivElement>();
            row.InnerText = _data[index].ToString();
            _viewPort.AppendChild(row);

            return row;
        }
    }
}