
using System;



namespace Aquila.QueryBuilder.Contracts
{
    /// <summary>
    /// Defines the column type
    /// </summary>
    /// <typeparam name="TNext">The interface to return after a type was specified</typeparam>
    public interface IColumnTypeSyntax<TNext>
    {



        /// <summary>
        /// Defines the column type as VarBinary
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsVarBinary(int size);

        /// <summary>
        /// Defines the column type as Binary
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
        /// Defines the column type as a <see cref="System.Guid"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsGuid();

        /// <summary>
        /// Defines the column type as a <see cref="float"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsFloat(int size, int precision);

        /// <summary>
        /// Defines the column type as a <see cref="short"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsSmallInt();

        /// <summary>
        /// Defines the column type as a <see cref="int"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsInt();

        /// <summary>
        /// Defines the column type as a <see cref="long"/>
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsBigInt();

        /// <summary>
        /// Defines the column type as unicode string
        /// </summary>
        /// <returns>The next step</returns>
        TNext AsString();


        /// <summary>
        /// Defines the column type as unicode string
        /// </summary>
        /// <param name="size">The maximum length in code points</param>
        /// <returns>The next step</returns>
        TNext AsString(int size);



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
