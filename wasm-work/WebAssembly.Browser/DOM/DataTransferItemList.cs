﻿using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;

using WebAssembly;

namespace WebAssembly.Browser.DOM
{
    [Export("DataTransferItemList", typeof(JSObject))]
    public sealed class DataTransferItemList : DOMObject
    {
        internal DataTransferItemList(JSObject handle) : base(handle) { }

        //public DataTransferItemList() { }
        [Export("length")]
        public double Length => GetProperty<double>("length");
        //[Export("add")]
        //public IDataTransferItem Add(File data)
        //{
        //    return InvokeMethod<DataTransferItem>("add", data);
        //}
        [Export("clear")]
        public void Clear()
        {
            InvokeMethod<object>("clear");
        }
        [Export("item")]
        public DataTransferItem Item(double index)
        {
            return InvokeMethod<DataTransferItem>("item", index);
        }
        [Export("remove")]
        public void Remove(double index)
        {
            InvokeMethod<object>("remove", index);
        }
        [IndexerName("TheItem")]
        public DataTransferItem this[double index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }}
