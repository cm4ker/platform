// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Aquila.CodeAnalysis
{
    internal partial class BoundSwitchStatement : IBoundSwitchStatement
    {
        BoundNode IBoundSwitchStatement.Value => this.Expression;
        ImmutableArray<BoundStatementList> IBoundSwitchStatement.Cases => StaticCast<BoundStatementList>.From(this.SwitchSections);
    }
}
