// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace Aquila.CodeAnalysis.Syntax.InternalSyntax
{
    internal readonly struct BlendedNode
    {
        internal readonly Aquila.CodeAnalysis.AquilaSyntaxNode Node;
        internal readonly SyntaxToken Token;
        internal readonly Blender Blender;

        internal BlendedNode(Aquila.CodeAnalysis.AquilaSyntaxNode node, SyntaxToken token, Blender blender)
        {
            this.Node = node;
            this.Token = token;
            this.Blender = blender;
        }
    }
}