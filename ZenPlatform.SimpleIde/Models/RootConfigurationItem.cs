﻿using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media.Imaging;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.SimpleIde.Models
{
    public class RootConfigurationItem : IConfiguratoinItem
    {
        private List<IConfiguratoinItem> _childs;
        private MDRoot _root;


        public RootConfigurationItem(MDRoot root)
        {
            _root = root;

           

        }

        public string Content => _root.ProjectName;

        public IBitmap Bitmap => null;

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public bool CanOpen => true;

        public bool HasContext => true;
        public object Context
        {
            get => _root;
            set
            {
                _root = value as MDRoot;

            }
        }

        public IEnumerable<IConfiguratoinItem> Childs => _childs;
    }
}
