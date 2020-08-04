﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

 using Microsoft.CodeAnalysis;

 namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Utility class for substituting actual type arguments for formal generic type parameters.
    /// </summary>
    internal sealed class MutableTypeMap : AbstractTypeParameterMap
    {
        internal MutableTypeMap()
            : base(new SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>())
        {
        }

        internal void Add(TypeParameterSymbol key, TypeWithAnnotations value)
        {
            this.Mapping.Add(key, value);
        }
    }
}
