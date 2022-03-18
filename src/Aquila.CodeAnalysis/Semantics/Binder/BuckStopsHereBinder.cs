// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// A binder that knows no symbols and will not delegate further.
    /// </summary>
    internal class BuckStopsHereBinder : Binder
    {
        internal BuckStopsHereBinder(AquilaCompilation compilation) : base(compilation)
        {
            
        }
    }
}