using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using ZenPlatform.UIGeneration2.System.Windows;

namespace ZenPlatform.UIGeneration2
{


    public enum Orientation
    {
        Horizontal,
        Vertical,
    }

    /// <summary>Describes the kind of value that a <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> object is holding. </summary>
    public enum GridUnitType
    {
        Auto,
        Pixel,
        Star,
    }

    /// <summary>Represents the length of elements that explicitly support <see cref="F:ZenPlatform.UIGeneration2.GridUnitType.Star" /> unit types. </summary>
//[TypeConverter(typeof(GridLengthConverter))]
    public struct GridLength : IEquatable<GridLength>
    {
        private static readonly GridLength s_auto = new GridLength(1.0, GridUnitType.Auto);
        private double _unitValue;
        private GridUnitType _unitType;

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.GridLength" /> structure using the specified absolute value in pixels. </summary>
        /// <param name="pixels">The number of device-independent pixels (96 pixels-per-inch).</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="Pixels" /> is equal to <see cref="F:System.Double.NegativeInfinity" />, <see cref="F:System.Double.PositiveInfinity" />, or <see cref="F:System.Double.NaN" />.</exception>
        public GridLength(double pixels)
        {
            this = new GridLength(pixels, GridUnitType.Pixel);
        }

        /// <summary>Initializes a new instance of the <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> structure and specifies what kind of value it holds. </summary>
        /// <param name="value">The initial value of this instance of <see cref="T:ZenPlatform.UIGeneration2.GridLength" />.</param>
        /// <param name="type">The <see cref="T:ZenPlatform.UIGeneration2.GridUnitType" /> held by this instance of <see cref="T:ZenPlatform.UIGeneration2.GridLength" />.</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="value" /> is equal to <see cref="F:System.Double.NegativeInfinity" />, <see cref="F:System.Double.PositiveInfinity" />, or <see cref="F:System.Double.NaN" />.</exception>
        public GridLength(double value, GridUnitType type)
        {

            this._unitValue = type == GridUnitType.Auto ? 0.0 : value;
            this._unitType = type;
        }

        /// <summary>Compares two <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> structures for equality.</summary>
        /// <returns>true if the two instances of <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> have the same value and <see cref="T:ZenPlatform.UIGeneration2.GridUnitType" />; otherwise, false.</returns>
        /// <param name="gl1">The first instance of <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> to compare.</param>
        /// <param name="gl2">The second instance of <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> to compare.</param>
        public static bool operator ==(GridLength gl1, GridLength gl2)
        {
            if (gl1.GridUnitType == gl2.GridUnitType)
                return gl1.Value == gl2.Value;
            return false;
        }

        /// <summary>Compares two <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> structures to determine if they are not equal.</summary>
        /// <returns>true if the two instances of <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> do not have the same value and <see cref="T:ZenPlatform.UIGeneration2.GridUnitType" />; otherwise, false.</returns>
        /// <param name="gl1">The first instance of <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> to compare.</param>
        /// <param name="gl2">The second instance of <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> to compare.</param>
        public static bool operator !=(GridLength gl1, GridLength gl2)
        {
            if (gl1.GridUnitType == gl2.GridUnitType)
                return gl1.Value != gl2.Value;
            return true;
        }

        /// <summary>Determines whether the specified object is equal to the current <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> instance. </summary>
        /// <returns>true if the specified object has the same value and <see cref="T:ZenPlatform.UIGeneration2.GridUnitType" /> as the current instance; otherwise, false.</returns>
        /// <param name="oCompare">The object to compare with the current instance.</param>
        public override bool Equals(object oCompare)
        {
            if (oCompare is GridLength)
                return this == (GridLength)oCompare;
            return false;
        }

        /// <summary>Determines whether the specified <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> is equal to the current <see cref="T:ZenPlatform.UIGeneration2.GridLength" />.</summary>
        /// <returns>true if the specified <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> has the same value and <see cref="P:ZenPlatform.UIGeneration2.GridLength.GridUnitType" /> as the current instance; otherwise, false.</returns>
        /// <param name="gridLength">The <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> structure to compare with the current instance.</param>
        public bool Equals(GridLength gridLength)
        {
            return this == gridLength;
        }

        /// <summary>Gets a hash code for the <see cref="T:ZenPlatform.UIGeneration2.GridLength" />. </summary>
        /// <returns>A hash code for the current <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> structure.</returns>
        public override int GetHashCode()
        {
            return (int)((int)this._unitValue + this._unitType);
        }

        /// <summary>Gets a value that indicates whether the <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> holds a value that is expressed in pixels. </summary>
        /// <returns>true if the <see cref="P:ZenPlatform.UIGeneration2.GridLength.GridUnitType" /> property is <see cref="F:ZenPlatform.UIGeneration2.GridUnitType.Pixel" />; otherwise, false.</returns>
        public bool IsAbsolute
        {
            get
            {
                return this._unitType == GridUnitType.Pixel;
            }
        }

        /// <summary>Gets a value that indicates whether the <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> holds a value whose size is determined by the size properties of the content object. </summary>
        /// <returns>true if the <see cref="P:ZenPlatform.UIGeneration2.GridLength.GridUnitType" /> property is <see cref="F:ZenPlatform.UIGeneration2.GridUnitType.Auto" />; otherwise, false. </returns>
        public bool IsAuto
        {
            get
            {
                return this._unitType == GridUnitType.Auto;
            }
        }

        /// <summary>Gets a value that indicates whether the <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> holds a value that is expressed as a weighted proportion of available space. </summary>
        /// <returns>true if the <see cref="P:ZenPlatform.UIGeneration2.GridLength.GridUnitType" /> property is <see cref="F:ZenPlatform.UIGeneration2.GridUnitType.Star" />; otherwise, false. </returns>
        public bool IsStar
        {
            get
            {
                return this._unitType == GridUnitType.Star;
            }
        }

        /// <summary>Gets a <see cref="T:System.Double" /> that represents the value of the <see cref="T:ZenPlatform.UIGeneration2.GridLength" />.</summary>
        /// <returns>A <see cref="T:System.Double" /> that represents the value of the current instance. </returns>
        public double Value
        {
            get
            {
                if (this._unitType != GridUnitType.Auto)
                    return this._unitValue;
                return 1.0;
            }
        }

        /// <summary>Gets the associated <see cref="T:ZenPlatform.UIGeneration2.GridUnitType" /> for the <see cref="T:ZenPlatform.UIGeneration2.GridLength" />. </summary>
        /// <returns>One of the <see cref="T:ZenPlatform.UIGeneration2.GridUnitType" /> values. The default is <see cref="F:ZenPlatform.UIGeneration2.GridUnitType.Auto" />.</returns>
        public GridUnitType GridUnitType
        {
            get
            {
                return this._unitType;
            }
        }

        /// <summary>Returns a <see cref="T:System.String" /> representation of the <see cref="T:ZenPlatform.UIGeneration2.GridLength" />.</summary>
        /// <returns>A <see cref="T:System.String" /> representation of the current <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> structure.</returns>
        public override string ToString()
        {
            return GridLengthConverter.ToString(this, CultureInfo.InvariantCulture);
        }

        /// <summary>Gets an instance of <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> that holds a value whose size is determined by the size properties of the content object.</summary>
        /// <returns>A instance of <see cref="T:ZenPlatform.UIGeneration2.GridLength" /> whose <see cref="P:ZenPlatform.UIGeneration2.GridLength.GridUnitType" /> property is set to <see cref="F:ZenPlatform.UIGeneration2.GridUnitType.Auto" />. </returns>
        public static GridLength Auto
        {
            get
            {
                return GridLength.s_auto;
            }
        }
    }

    internal static class DoubleUtil
    {
        internal const double DBL_EPSILON = 2.22044604925031E-16;
        internal const float FLT_MIN = 1.175494E-38f;

        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
                return true;
            double num1 = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.22044604925031E-16;
            double num2 = value1 - value2;
            if (-num1 < num2)
                return num1 > num2;
            return false;
        }

        public static bool LessThan(double value1, double value2)
        {
            if (value1 < value2)
                return !DoubleUtil.AreClose(value1, value2);
            return false;
        }

        public static bool GreaterThan(double value1, double value2)
        {
            if (value1 > value2)
                return !DoubleUtil.AreClose(value1, value2);
            return false;
        }

        public static bool LessThanOrClose(double value1, double value2)
        {
            if (value1 >= value2)
                return DoubleUtil.AreClose(value1, value2);
            return true;
        }

        public static bool GreaterThanOrClose(double value1, double value2)
        {
            if (value1 <= value2)
                return DoubleUtil.AreClose(value1, value2);
            return true;
        }

        public static bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < 2.22044604925031E-15;
        }

        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 2.22044604925031E-15;
        }

        public static bool AreClose(Point point1, Point point2)
        {
            if (DoubleUtil.AreClose(point1.X, point2.X))
                return DoubleUtil.AreClose(point1.Y, point2.Y);
            return false;
        }

        public static bool AreClose(Size size1, Size size2)
        {
            if (DoubleUtil.AreClose(size1.Width, size2.Width))
                return DoubleUtil.AreClose(size1.Height, size2.Height);
            return false;
        }


        public static bool IsBetweenZeroAndOne(double val)
        {
            if (DoubleUtil.GreaterThanOrClose(val, 0.0))
                return DoubleUtil.LessThanOrClose(val, 1.0);
            return false;
        }

        public static int DoubleToInt(double val)
        {
            if (0.0 >= val)
                return (int)(val - 0.5);
            return (int)(val + 0.5);
        }


        public static bool IsNaN(double value)
        {
            DoubleUtil.NanUnion nanUnion = new DoubleUtil.NanUnion();
            nanUnion.DoubleValue = value;
            ulong num1 = nanUnion.UintValue & 18442240474082181120UL;
            ulong num2 = nanUnion.UintValue & 4503599627370495UL;
            if ((long)num1 == 9218868437227405312L || (long)num1 == -4503599627370496L)
                return num2 > 0UL;
            return false;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal ulong UintValue;
        }
    }





    namespace System.Windows
    {
        /// <summary>Converts instances of other types to and from <see cref="T:System.Windows.GridLength" /> instances. </summary>
        public class GridLengthConverter : TypeConverter
        {
            /// <summary>Determines whether a class can be converted from a given type to an instance of <see cref="T:System.Windows.GridLength" />.</summary>
            /// <returns>true if the converter can convert from the specified type to an instance of <see cref="T:System.Windows.GridLength" />; otherwise, false.</returns>
            /// <param name="typeDescriptorContext">Describes the context information of a type.</param>
            /// <param name="sourceType">The type of the source that is being evaluated for conversion.</param>
            public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
            {
                switch (Type.GetTypeCode(sourceType))
                {
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.String:
                        return true;
                    default:
                        return false;
                }
            }

            /// <summary>Determines whether an instance of <see cref="T:System.Windows.GridLength" /> can be converted to a different type. </summary>
            /// <returns>true if the converter can convert this instance of <see cref="T:System.Windows.GridLength" /> to the specified type; otherwise, false.</returns>
            /// <param name="typeDescriptorContext">Describes the context information of a type.</param>
            /// <param name="destinationType">The desired type that this instance of <see cref="T:System.Windows.GridLength" /> is being evaluated for conversion.</param>
            public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
            {
                if (!(destinationType == typeof(InstanceDescriptor)))
                    return destinationType == typeof(string);
                return true;
            }

            /// <summary>Attempts to convert a specified object to an instance of <see cref="T:System.Windows.GridLength" />. </summary>
            /// <returns>The instance of <see cref="T:System.Windows.GridLength" /> that is created from the converted <paramref name="source" />.</returns>
            /// <param name="typeDescriptorContext">Describes the context information of a type.</param>
            /// <param name="cultureInfo">Cultural specific information that should be respected during conversion.</param>
            /// <param name="source">The object being converted.</param>
            /// <exception cref="T:System.ArgumentNullException">
            /// <paramref name="source" /> object is null.</exception>
            /// <exception cref="T:System.ArgumentException">
            /// <paramref name="source" /> object is not null and is not a valid type that can be converted to a <see cref="T:System.Windows.GridLength" />.</exception>
            public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
            {
                if (source == null)
                    throw this.GetConvertFromException(source);
                if (source is string)
                    return (object)GridLengthConverter.FromString((string)source, cultureInfo);
                double num = Convert.ToDouble(source, (IFormatProvider)cultureInfo);
                GridUnitType type;
                if (DoubleUtil.IsNaN(num))
                {
                    num = 1.0;
                    type = GridUnitType.Auto;
                }
                else
                    type = GridUnitType.Pixel;
                return (object)new GridLength(num, type);
            }

            /// <summary>Attempts to convert an instance of <see cref="T:System.Windows.GridLength" /> to a specified type. </summary>
            /// <returns>The object that is created from the converted instance of <see cref="T:System.Windows.GridLength" />.</returns>
            /// <param name="typeDescriptorContext">Describes the context information of a type.</param>
            /// <param name="cultureInfo">Cultural specific information that should be respected during conversion.</param>
            /// <param name="value">The instance of <see cref="T:System.Windows.GridLength" /> to convert.</param>
            /// <param name="destinationType">The type that this instance of <see cref="T:System.Windows.GridLength" /> is converted to.</param>
            /// <exception cref="T:System.ArgumentNullException">
            /// <paramref name="destinationType" /> is not one of the valid types for conversion.</exception>
            /// <exception cref="T:System.ArgumentException">
            /// <paramref name="value" /> is null.</exception>
            [SecurityCritical]
            public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
            {
                if (destinationType == (Type)null)
                    throw new ArgumentNullException(nameof(destinationType));
                if (value != null && value is GridLength)
                {
                    GridLength gl = (GridLength)value;
                    if (destinationType == typeof(string))
                        return (object)GridLengthConverter.ToString(gl, cultureInfo);
                    if (destinationType == typeof(InstanceDescriptor))
                        return (object)new InstanceDescriptor((MemberInfo)typeof(GridLength).GetConstructor(new Type[2]
                        {
            typeof (double),
            typeof (GridUnitType)
                        }), (ICollection)new object[2]
                        {
            (object) gl.Value,
            (object) gl.GridUnitType
                        });
                }
                throw this.GetConvertToException(value, destinationType);
            }

            internal static string ToString(GridLength gl, CultureInfo cultureInfo)
            {
                switch (gl.GridUnitType)
                {
                    case GridUnitType.Auto:
                        return "Auto";
                    case GridUnitType.Star:
                        if (!DoubleUtil.IsOne(gl.Value))
                            return Convert.ToString(gl.Value, (IFormatProvider)cultureInfo) + "*";
                        return "*";
                    default:
                        return Convert.ToString(gl.Value, (IFormatProvider)cultureInfo);
                }
            }

            internal static GridLength FromString(string s, CultureInfo cultureInfo)
            {
                throw new NotImplementedException();
                double num;
                GridUnitType unit;
                ///XamlGridLengthSerializer.FromString(s, cultureInfo, out num, out unit);
                return new GridLength(num, unit);
            }
        }
    }

}