using System;
using System.Collections.Generic;
using System.Text;
using Aquila.CodeAnalysis.Semantics;

namespace Aquila.CodeAnalysis.Utilities
{
    /// <summary>
    /// Empty structure to be used in generic classes requiring return or argument type (such as <see cref="AquilaOperationVisitor{TResult}"/>).
    /// </summary>
    public struct VoidStruct
    {
    }
}
