// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Aquila.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis
{
    public partial class CSharpSyntaxTree
    {
        private class DebuggerSyntaxTree : ParsedSyntaxTree
        {
            public DebuggerSyntaxTree(CSharpSyntaxNode root, SourceText text, AquilaParseOptions options)
                : base(
                    text,
                    text.Encoding,
                    text.ChecksumAlgorithm,
                    path: "",
                    options: options,
                    root: root,
                    directives: new object(),
                    diagnosticOptions: null,
                    cloneRoot: true)
            {
            }

            internal override bool SupportsLocations
            {
                get { return true; }
            }
        }
    }
}