

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Define the sequence options
    /// </summary>
    public interface ICreateSequenceSyntax
    {
        /// <summary>
        /// Defines the increment
        /// </summary>
        /// <param name="increment">The value used to increment the sequence</param>
        /// <returns>Define the sequence options</returns>
        ICreateSequenceSyntax IncrementBy(long increment);

        /// <summary>
        /// Sets the minimum value of the sequence
        /// </summary>
        /// <param name="minValue">The minimum value of the sequence</param>
        /// <returns>Define the sequence options</returns>
        ICreateSequenceSyntax MinValue(long minValue);

        /// <summary>
        /// Sets the maximum value of the sequence
        /// </summary>
        /// <param name="maxValue">The maximum value of the sequence</param>
        /// <returns>Define the sequence options</returns>
        ICreateSequenceSyntax MaxValue(long maxValue);

        /// <summary>
        /// Sets the start value of the sequence
        /// </summary>
        /// <param name="startwith">The start value</param>
        /// <returns>Define the sequence options</returns>
        ICreateSequenceSyntax StartWith(long startwith);

        /// <summary>
        /// Cache the next <paramref name="value"/> number of values for a single sequence increment
        /// </summary>
        /// <remarks>Normally used together with <see cref="IncrementBy"/></remarks>
        /// <param name="value">The number of values to cache</param>
        /// <returns>Define the sequence options</returns>
        ICreateSequenceSyntax Cache(long value);

        /// <summary>
        /// Defines that the sequence starts again with the <see cref="MinValue"/> value for the next value after <see cref="MaxValue"/>
        /// </summary>
        /// <returns>Define the sequence options</returns>
        ICreateSequenceSyntax Cycle();
    }
}
