﻿using System;
 using System.Collections.Immutable;
 using System.Text.RegularExpressions;
 using Aquila.CodeAnalysis;
 using Aquila.CodeAnalysis.Errors;
 using Aquila.CodeAnalysis.Semantics;

 namespace Aquila.CodeAnalysis.FlowAnalysis.Passes
{
    internal partial class DiagnosticWalker<T>
    {
        /// <summary>
        /// Matches <c>printf()</c> format specifier.
        /// </summary>
        static readonly Lazy<Regex> s_printfSpecsRegex = new Lazy<Regex>(
            () => new Regex(@"%(?:(\d)+\$)?[+-]?(?:[ 0]|'.{1})?-?\d*(?:\.\d+)?[bcdeEufFgGosxX]", RegexOptions.Compiled | RegexOptions.CultureInvariant)
        );

        void printfCheck(string name, ImmutableArray<BoundArgument> arguments)
        {
            // Check that the number of arguments matches the format string
            if (arguments.Length != 0 &&
                arguments[0].Value.ConstantValue.TryConvertToString(out string format))
            {
                int posSpecCount = 0;
                int numSpecMax = 0;
                foreach (Match match in s_printfSpecsRegex.Value.Matches(format))
                {
                    var numSpecStr = match.Groups[1].Value;
                    if (numSpecStr == string.Empty)
                    {
                        // %d
                        posSpecCount++;
                    }
                    else
                    {
                        // %2$d
                        int numSpec = int.Parse(numSpecStr);
                        numSpecMax = Math.Max(numSpec, numSpecMax);
                    }
                }

                int expectedArgCount = 1 + Math.Max(posSpecCount, numSpecMax);

                if (arguments.Length != expectedArgCount)
                {
                    // Wrong number of arguments with respect to the format string

                    _diagnostics.Add(
                        _method, arguments[0].Value.GetTextSpan(),
                        ErrorCode.WRN_FormatStringWrongArgCount, name.ToLowerInvariant(), expectedArgCount, arguments.Length);
                }
            }
        }
    }
}
