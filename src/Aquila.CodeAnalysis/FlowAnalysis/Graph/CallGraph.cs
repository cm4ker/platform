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
        public CallSite(BoundBlock block, BoundCall callExpression)
        {
            Block = block;
            CallExpression = callExpression;
        }

        BoundBlock Block { get; }

        BoundCall CallExpression { get; }
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
        private readonly ConcurrentDictionary<SourceMethodSymbol, ConcurrentBag<Edge>> _incidentEdges;

        public CallGraph()
        {
            _incidentEdges = new ConcurrentDictionary<SourceMethodSymbol, ConcurrentBag<Edge>>();
        }

        public Edge AddEdge(SourceMethodSymbol caller, SourceMethodSymbol callee, CallSite callSite)
        {
            var edge = new Edge(caller, callee, callSite);
            AddMethodEdge(caller, edge);
            AddMethodEdge(callee, edge);

            return edge;
        }

        public IEnumerable<Edge> GetIncidentEdges(SourceMethodSymbol method)
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

        public IEnumerable<Edge> GetCalleeEdges(SourceMethodSymbol caller)
        {
            return GetIncidentEdges(caller).Where(edge => edge.Caller == caller);
        }

        public IEnumerable<Edge> GetCallerEdges(SourceMethodSymbol callee)
        {
            return GetIncidentEdges(callee).Where(edge => edge.Callee == callee);
        }

        private void AddMethodEdge(SourceMethodSymbol method, Edge edge)
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
            public Edge(SourceMethodSymbol caller, SourceMethodSymbol callee, CallSite callSite)
            {
                Caller = caller;
                Callee = callee;
                CallSite = callSite;
            }

            public SourceMethodSymbol Caller { get; }

            public SourceMethodSymbol Callee { get; }

            public CallSite CallSite { get; }
        }
    }
}