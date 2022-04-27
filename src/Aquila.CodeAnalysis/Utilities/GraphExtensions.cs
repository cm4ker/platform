using System;
using System.Collections.Generic;
using System.Text;
using Aquila.CodeAnalysis.Semantics.Graph;

namespace Aquila.CodeAnalysis
{
    // internal static class GraphExtensions
    // {
    //     internal static TBlock WithLocalPropertiesFrom<TBlock>(this TBlock self, TBlock other)
    //         where TBlock : BoundBlock
    //     {
    //         self.Tag = other.Tag;
    //         self.Ordinal = other.Ordinal;
    //         self.FlowState = other.FlowState?.Clone();
    //
    //         return self;
    //     }
    //
    //     internal static TBlock WithEdge<TBlock>(this TBlock self, Edge edge)
    //         where TBlock : BoundBlock
    //     {
    //         self.SetNextEdge(edge);
    //         return self;
    //     }
    // }
}