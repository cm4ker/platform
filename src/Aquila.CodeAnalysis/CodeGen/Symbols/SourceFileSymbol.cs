﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
 using Pchp.CodeAnalysis.Emit;

 namespace Aquila.CodeAnalysis.Symbols
{
    partial class SourceFileSymbol
    {
        internal void SynthesizeInit(PEModuleBuilder module, DiagnosticBag diagnostics)
        {
            // module.EmitBootstrap(this); // unnecessary
        }
    }
}
