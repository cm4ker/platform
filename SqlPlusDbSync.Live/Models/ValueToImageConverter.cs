﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace SqlPlusDbSync.Live.Models
{
    public sealed class ValueToImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int)) return null;
            return ImageLoader.GetImageById((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Image to Id conversion is not supported!");
        }

        #endregion
    }
}