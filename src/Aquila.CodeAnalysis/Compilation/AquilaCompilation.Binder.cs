using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis.Semantics;
using Aquila.Syntax;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis
{
    public partial class AquilaCompilation
    {
        internal Binder GetBinder(AquilaSyntaxNode syntax)
        {
            return BinderFactory.Visit(syntax);
        }
    }
}