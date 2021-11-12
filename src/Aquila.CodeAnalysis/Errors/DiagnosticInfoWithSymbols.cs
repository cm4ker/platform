// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Immutable;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis
{
    internal class DiagnosticInfoWithSymbols : DiagnosticInfo
    {
        // not serialized:
        internal readonly ImmutableArray<Symbol> Symbols;

        internal DiagnosticInfoWithSymbols(ErrorCode errorCode, object[] arguments, ImmutableArray<Symbol> symbols)
            : base(Aquila.CodeAnalysis.Errors.MessageProvider.Instance, (int)errorCode, arguments)
        {
            this.Symbols = symbols;
        }

        internal DiagnosticInfoWithSymbols(bool isWarningAsError, ErrorCode errorCode, object[] arguments,
            ImmutableArray<Symbol> symbols)
            : base(Aquila.CodeAnalysis.Errors.MessageProvider.Instance, isWarningAsError, (int)errorCode, arguments)
        {
            this.Symbols = symbols;
        }
    }
}