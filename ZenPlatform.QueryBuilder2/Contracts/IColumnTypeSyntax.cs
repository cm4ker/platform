
using System;



namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Defines the column type
    /// </summary>
    /// <typeparam name="TNext">The interface to return after a type was specified</typeparam>
    public interface IColumnTypeSyntax<TNext>
    {
        /// <summary>
        /// Defines the column type as ANSI string (single byte character set)
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsAnsiString();

        /// <summary>
        /// Defines the column type as ANSI string (single byte character set) with the given <paramref name="collationName"/>
        /// </summary>
        /// <param name="collationName">The collation to use for this column</param>
        /// <returns>The next step</returns>
        TNext AsAnsiString(string collationName);

        /// <summary>
        /// Defines the column type as ANSI string (single byte character set) with the given <paramref name="size"/>
        /// </summary>
        /// <param name="size">The maximum size (usually in bytes) of the ansi string</param>
        /// <returns>The next step</returns>
        TNext AsAnsiString(int size);

        /// <summary>
        /// Defines the column type as ANSI string (single byte character set) with the given <paramref name="size"/> and <paramref name="collationName"/>
        /// </summary>
        /// <param name="size">The maximum size (usually in bytes) of the ansi string</param>
        /// <param name="collationName">The collation to use for this column</param>
        /// <returns>The next step</returns>
        TNext AsAnsiString(int size, string collationName);

        /// <summary>
        /// Defines the column type as BLOB
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsBinary();

        /// <summary>
        /// Defines the column type as BLOB
        /// </summary>
        /// <param name="size">The maximum size of the BLOB (in bytes)</param>
        /// <returns>The next step</returns>
        TNext AsBinary(int size);

        /// <summary>
        /// Defines the column type as <see cref="bool"/> (or bit)
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsBoolean();

        /// <summary>
        /// Defines the column type as <see cref="byte"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsByte();

        /// <summary>
        /// Defines the column type as currency
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsCurrency();

        /// <summary>
        /// Defines the column type as date
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsDate();

        /// <summary>
        /// Defines the column type as <see cref="DateTime"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsDateTime();


        /// <summary>
        /// Defines the column type as <see cref="DateTime"/> with extended range and precision
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsDateTime2();

        /// <summary>
        /// Defines the column type as <see cref="DateTimeOffset"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsDateTimeOffset();

        /// <summary>
        /// Defines the column type as <see cref="DateTimeOffset"/>
        /// </summary>
        /// <param name="precision">The number of digits for the fraction of a second</param>
        /// <returns>The next step</returns>
        TNext AsDateTimeOffset(int precision);

        /// <summary>
        /// Defines the column type as <see cref="decimal"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsDecimal();

        /// <summary>
        /// Defines the column type as decimal with given size and precision
        /// </summary>
        /// <param name="size">The number of digits</param>
        /// <param name="precision">The number of digits after the comma</param>
        /// <returns>The next step</returns>
        TNext AsDecimal(int size, int precision);

        /// <summary>
        /// Defines the column type as <see cref="double"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsDouble();

        /// <summary>
        /// Defines the column type as a <see cref="System.Guid"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsGuid();

        /// <summary>
        /// Defines the column type as unicode string with a fixed length
        /// </summary>
        /// <param name="size">The maximum length of the string in code points</param>
        /// <returns>The next step</returns>
        TNext AsFixedLengthString(int size);

        /// <summary>
        /// Defines the column type as unicode string with a fixed length
        /// </summary>
        /// <param name="size">The length of the string in code points</param>
        /// <param name="collationName">The name of the collation to use</param>
        /// <returns>The next step</returns>
        TNext AsFixedLengthString(int size, string collationName);

        /// <summary>
        /// Defines the column type as ANSI string with fixed length
        /// </summary>
        /// <param name="size">The length of the string in bytes</param>
        /// <returns>The next step</returns>
        TNext AsFixedLengthAnsiString(int size);

        /// <summary>
        /// Defines the column type as ANSI string with fixed length
        /// </summary>
        /// <param name="size">The length of the string in bytes</param>
        /// <param name="collationName">The name of the collation to use</param>
        /// <returns>The next step</returns>
        TNext AsFixedLengthAnsiString(int size, string collationName);

        /// <summary>
        /// Defines the column type as a <see cref="float"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsFloat();

        /// <summary>
        /// Defines the column type as a <see cref="short"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsInt16();

        /// <summary>
        /// Defines the column type as a <see cref="int"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsInt32();

        /// <summary>
        /// Defines the column type as a <see cref="long"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsInt64();

        /// <summary>
        /// Defines the column type as unicode string
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsString();

        /// <summary>
        /// Defines the column type as unicode string
        /// </summary>
        /// <param name="collationName">The name of the collation</param>
        /// <returns>The next step</returns>
        TNext AsString(string collationName);

        /// <summary>
        /// Defines the column type as unicode string
        /// </summary>
        /// <param name="size">The maximum length in code points</param>
        /// <returns>The next step</returns>
        TNext AsString(int size);

        /// <summary>
        /// Defines the column type as unicode string
        /// </summary>
        /// <param name="size">The maximum length in code points</param>
        /// <param name="collationName">The name of the collation</param>
        /// <returns>The next step</returns>
        TNext AsString(int size, string collationName);

        /// <summary>
        /// Defines the column type as <see cref="TimeSpan"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsTime();

        /// <summary>
        /// Defines the column type as XML
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsXml();

        /// <summary>
        /// Defines the column type as XML
        /// </summary>
        /// <param name="size">The maximum size</param>
        /// <returns>The next step</returns>
        TNext AsXml(int size);

        /// <summary>
        /// Defines the column with a custom (DB-specific) type
        /// </summary>
        /// <param name="customType">The custom type as SQL identifier</param>
        /// <returns>The next step</returns>
        TNext AsCustom(string customType);
    }
}
