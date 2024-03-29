﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquila.CodeAnalysis.FlowAnalysis
{
    /// <summary>
    /// Compatible Aquila type codes.
    /// </summary>
    public enum AquilaTypeCode
    {
        /// <summary>
        /// An invalid value, <c>void</c>.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// An invalid value, <c>void</c>.
        /// </summary>
        Void = Undefined,

        /// <summary>
        /// The value is of type boolean.
        /// </summary>
        Boolean,

        /// <summary>
        /// 32-bit integer value.
        /// </summary>
        Int,

        /// <summary>
        /// 64-bit integer value.
        /// </summary>
        Long,

        /// <summary>
        /// 64-bit floating point number.
        /// </summary>
        Double,

        /// <summary>
        /// Unicode string value. Two-byte (UTF16) readonly string.
        /// </summary>
        String,

        /// <summary>
        /// Both Unicode and Binary writable string value. Encapsulates two-byte (UTF16), single-byte (binary) string and string builder.
        /// </summary>
        WritableString,

        /// <summary>
        /// A Aquila array.
        /// </summary>
        AquilaArray,

        /// <summary>
        /// A class type, including <c>NULL</c>, <c>Closure</c> or a generic <c>Object</c>.
        /// </summary>
        Object,

        /// <summary>
        /// A Aquila resource.
        /// </summary>
        Resource,

        /// <summary>
        /// An iterable object (array, Traversable).
        /// </summary>
        Iterable,

        /// <summary>
        /// Callable type (array(2), string, object).
        /// </summary>
        Callable,

        /// <summary>
        /// Object that might be <c>NULL</c>.
        /// Used in combination with <see cref="Object"/> (?).
        /// </summary>
        Null,

        /// <summary>
        /// Any type, used for compatibility with <c>mixed</c> primitive type.
        /// </summary>
        Mixed,
    }
}