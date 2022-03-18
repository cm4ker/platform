// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace Aquila.CodeAnalysis.Syntax.InternalSyntax
{
    internal abstract partial class TypeEx
    {
        public bool IsVar => IsIdentifierName("var");
        public bool IsUnmanaged => IsIdentifierName("unmanaged");
        public bool IsNotNull => IsIdentifierName("notnull");
        public bool IsNint => IsIdentifierName("nint");
        public bool IsNuint => IsIdentifierName("nuint");

        private bool IsIdentifierName(string id) =>
            this is IdentifierEx name && name.Identifier.ToString() == id;

        public bool IsRef => Kind == SyntaxKind.RefType;
    }
}