using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;

namespace Aquila.CodeAnalysis.FlowAnalysis.Graph
{
    struct CallSite
    {
        public CallSite(BoundBlock block, BoundCallEx callExpression)
        {
            Block = block;
            CallExpression = callExpression;
        }

        BoundBlock Block { get; }

        BoundCallEx CallExpression { get; }
    }

    /// <summary>
    /// Stores the information about the calls among the methods in source code. This class is thread-safe.
    /// </summary>
    class CallGraph
    {
        /// <summary>
        /// Maps each node to its incident edges, their directions can be found by <see cref="Edge.Caller"/>
        /// and <see cref="Edge.Callee"/>.
        /// </summary>
        private readonly ConcurrentDictionary<SourceMethodSymbolBase, ConcurrentBag<Edge>> _incidentEdges;

        public CallGraph()
        {
            _incidentEdges = new ConcurrentDictionary<SourceMethodSymbolBase, ConcurrentBag<Edge>>();
        }

        public Edge AddEdge(SourceMethodSymbolBase caller, SourceMethodSymbolBase callee, CallSite callSite)
        {
            var edge = new Edge(caller, callee, callSite);
            AddMethodEdge(caller, edge);
            AddMethodEdge(callee, edge);

            return edge;
        }

        public IEnumerable<Edge> GetIncidentEdges(SourceMethodSymbolBase method)
        {
            if (_incidentEdges.TryGetValue(method, out var edges))
            {
                return edges;
            }
            else
            {
                return Array.Empty<Edge>();
            }
        }

        public IEnumerable<Edge> GetCalleeEdges(SourceMethodSymbolBase caller)
        {
            return GetIncidentEdges(caller).Where(edge => edge.Caller == caller);
        }

        public IEnumerable<Edge> GetCallerEdges(SourceMethodSymbolBase callee)
        {
            return GetIncidentEdges(callee).Where(edge => edge.Callee == callee);
        }

        private void AddMethodEdge(SourceMethodSymbolBase method, Edge edge)
        {
            _incidentEdges.AddOrUpdate(
                method,
                _ => new ConcurrentBag<Edge>() { edge },
                (_, edges) =>
                {
                    edges.Add(edge);
                    return edges;
                });
        }

        public class Edge
        {
            public Edge(SourceMethodSymbolBase caller, SourceMethodSymbolBase callee, CallSite callSite)
            {
                Caller = caller;
                Callee = callee;
                CallSite = callSite;
            }

            public SourceMethodSymbolBase Caller { get; }

            public SourceMethodSymbolBase Callee { get; }

            public CallSite CallSite { get; }
        }
    }
}