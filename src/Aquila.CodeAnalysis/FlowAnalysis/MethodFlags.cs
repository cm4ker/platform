﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquila.CodeAnalysis.FlowAnalysis
{
    /// <summary>
    /// Flags describing special method needs.
    /// Collected during flow analysis.
    /// </summary>
    [Flags]
    public enum MethodFlags
    {
        None = 0,

        HasEval = 1,
        HasInclude = 2,
        HasIndirectVar = 4,
        UsesLocals = 8,
        UsesArgs = 16,

        IsGenerator = 32,

        /// <summary>
        /// The method uses <c>static::</c> construct to access late static bound type.
        /// </summary>
        UsesLateStatic = 64,

        /// <summary>
        /// Indicates the method contains function calls.
        /// This can be used for analysis heuristics.
        /// </summary>
        HasUserFunctionCall = 128,

        /// <summary>
        /// Indicates that the method declaration was proven unreachable during the analysis.
        /// </summary>
        IsUnreachable = 256,

        /// <summary>The global function has been declared conditionally but analysis marked it as unconditional.</summary>
        MarkedDeclaredUnconditionally = 512,

        /// <summary>
        /// Internal. Marks whether the exit block or any block with a return statement was already processed at least once.
        /// </summary>
        IsReturnAnalysed = 1024,

        /// <summary>
        /// Whether the method has to define local variables as an array instead of native local variables.
        /// </summary>
        RequiresLocalsArray = HasEval | HasInclude | HasIndirectVar | UsesLocals | IsGenerator,

        /// <summary>
        /// Whether the method accesses its arguments dynamically we should provide params.
        /// </summary>
        RequiresVarArg = UsesArgs,
    }
}
