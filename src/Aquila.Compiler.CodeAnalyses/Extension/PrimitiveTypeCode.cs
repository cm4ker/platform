namespace Aquila.Language.Ast.Extension
{
    /// <summary>
    /// A enumeration of all of the value types that are built into the Runtime (and thus have specialized IL instructions that manipulate them).
    /// </summary>
    internal enum PrimitiveTypeCode
    {
        /// <summary>
        /// A single bit.
        /// </summary>
        Boolean,

        /// <summary>
        /// An unsigned 16 bit integer representing a Unicode UTF16 code point.
        /// </summary>
        Char,

        /// <summary>
        /// A signed 8 bit integer.
        /// </summary>
        Int8,

        /// <summary>
        /// A 32 bit IEEE floating point number.
        /// </summary>
        Float32,

        /// <summary>
        /// A 64 bit IEEE floating point number.
        /// </summary>
        Float64,

        /// <summary>
        /// A signed 16 bit integer.
        /// </summary>
        Int16,

        /// <summary>
        /// A signed 32 bit integer.
        /// </summary>
        Int32,

        /// <summary>
        /// A signed 64 bit integer.
        /// </summary>
        Int64,

        /// <summary>
        /// A signed 32 bit integer or 64 bit integer, depending on the native word size of the underlying processor.
        /// </summary>
        IntPtr,

        /// <summary>
        /// A pointer to fixed or unmanaged memory.
        /// </summary>
        Pointer,

        /// <summary>
        /// A reference to managed memory.
        /// </summary>
        Reference,

        /// <summary>
        /// A string.
        /// </summary>
        String,

        /// <summary>
        /// An unsigned 8 bit integer.
        /// </summary>
        UInt8,

        /// <summary>
        /// An unsigned 16 bit integer.
        /// </summary>
        UInt16,

        /// <summary>
        /// An unsigned 32 bit integer.
        /// </summary>
        UInt32,

        /// <summary>
        /// An unsigned 64 bit integer.
        /// </summary>
        UInt64,

        /// <summary>
        /// An unsigned 32 bit integer or 64 bit integer, depending on the native word size of the underlying processor.
        /// </summary>
        UIntPtr,

        /// <summary>
        /// A type that denotes the absence of a value.
        /// </summary>
        Void,

        /// <summary>
        /// Not a primitive type.
        /// </summary>
        NotPrimitive,

        /// <summary>
        /// A pointer to a function in fixed or managed memory.
        /// </summary>
        FunctionPointer,

        /// <summary>
        /// Type is a dummy type.
        /// </summary>
        Invalid,
    }
}