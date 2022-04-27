﻿using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.FlowAnalysis.Graph;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols.Aquila;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.FlowAnalysis
{
    /// <summary>
    /// Queue of work items to do.
    /// </summary>
    [DebuggerDisplay("WorkList<{T}>, Size={_queue.Count}")]
    internal class Worklist
    {
        readonly object _syncRoot = new object();

        /// <summary>
        /// Delegate used to process <typeparamref name="T"/>.
        /// </summary>
        public delegate void AnalyzeBlockDelegate(BoundBlock block);

        /// <summary>
        /// Action performed on bound operations.
        /// </summary>
        readonly List<AnalyzeBlockDelegate> _analyzers = new List<AnalyzeBlockDelegate>();

        //public event EventHandler MethodDone;

        ///// <summary>
        ///// Set of blocks being analyzed.
        ///// Used for recursion prevention.
        ///// </summary>
        //readonly HashSet<T> _pending;

        /// <summary>
        /// List of blocks to be processed.
        /// </summary>
        readonly DistinctQueue<BoundBlock> _queue = new DistinctQueue<BoundBlock>(new BoundBlockComparer());

        readonly CallGraph _callGraph = new CallGraph();

        /// <summary>
        /// Set of blocks that need to be processed, but the methods they call haven't been processed yet.
        /// </summary>
        readonly ConcurrentDictionary<BoundBlock, object> _dirtyCallBlocks = new ConcurrentDictionary<BoundBlock, object>();

        /// <summary>
        /// In the case of updating an existing analysis, a map of the currently analysed methods to their previous return types.
        /// Null in the case of a fresh analysis.
        /// </summary>
        Dictionary<SourceMethodSymbol, bool> _currentMethodsLastReturnTypes;

        ///// <summary>
        ///// Adds an analysis driver into the list of analyzers to be performed on bound operations.
        ///// </summary>
        //internal void AddAnalysis(AnalyzeBlockDelegate analyzer)
        //{
        //    _analyzers.Add(analyzer);
        //}

        public Worklist(params AnalyzeBlockDelegate[] analyzers)
        {
            _analyzers.AddRange(analyzers);
        }

        /// <summary>
        /// Adds block to the queue.
        /// </summary>
        public void Enqueue(BoundBlock block)
        {
            if (block != null)
            {
                _dirtyCallBlocks.TryRemove(block, out _);
                _queue.Enqueue(block);
            }
        }

        public bool EnqueueMethod(IAquilaMethodSymbol method, BoundBlock caller, BoundCall callExpression)
        {
            Contract.ThrowIfNull(method);

            if (method.ControlFlowGraph == null)
            {
                var method2 = method is SynthesizedMethodSymbol sr
                    ? sr.ForwardedCall
                    : method.OriginalDefinition as IAquilaMethodSymbol;

                if (method2 != null && !ReferenceEquals(method, method2))
                {
                    return EnqueueMethod(method2, caller, callExpression);
                }

                // library (sourceless) function
                return false;
            }

            var sourceMethod = (SourceMethodSymbol)method;

            if (sourceMethod.SyntaxReturnType != null)
            {
                // we don't have to wait for return type,
                // nor reanalyse itself when method analyses
                return false;
            }

            //_callGraph.AddEdge(caller.Method, sourceMethod, new CallSite(caller, callExpression));

            // ensure caller is subscribed to method's ExitBlock
            //((ExitBlock)method.ControlFlowGraph.Exit).Subscribe(caller);

            // TODO: check if method has to be reanalyzed => enqueue method's StartBlock

            // Return whether the method exit block will certainly be analysed in the future
            return !sourceMethod.IsReturnAnalysed;
        }

        // public void PingReturnUpdate(ExitBlock updatedExit, BoundBlock callingBlock)
        // {
        //     // var caller = callingBlock.FlowState?.Method;
        //     //
        //     // // If the update of the analysis is in progress and the caller is not yet analysed (its FlowState is null due to invalidation) or
        //     // // is not within the currently analysed methods, don't enqueue it
        //     // if (callingBlock.FlowState == null ||
        //     //     (caller != null && _currentMethodsLastReturnTypes != null &&
        //     //      !_currentMethodsLastReturnTypes.ContainsKey(caller)))
        //     // {
        //     //     return;
        //     // }
        //     //
        //     // if (caller == null || _callGraph.GetCalleeEdges(caller).All(edge => edge.Callee.IsReturnAnalysed))
        //     // {
        //     //     Enqueue(callingBlock);
        //     // }
        //     // else
        //     // {
        //     //     _dirtyCallBlocks.TryAdd(callingBlock, null);
        //     // }
        // }

        void Process(BoundBlock block)
        {
            var list = _analyzers;
            for (int i = 0; i < list.Count; i++)
            {
                list[i](block);
            }

            //CompilerLogSource.Log.Count("BoundBlockProcessings");
        }

        /// <summary>
        /// Processes all tasks until the queue is not empty.
        /// </summary>
        public void DoAll(bool concurrent = false)
        {
            // Store the current batch and its count
            var todoBlocks = new BoundBlock[256];
            int n;

            // Deque a batch of blocks and analyse them in parallel
            while (true)
            {
                n = Dequeue(todoBlocks);

                if (n == 0)
                {
                    if (_dirtyCallBlocks.IsEmpty)
                    {
                        break;
                    }
                    else
                    {
                        // Process also the call blocks that weren't analysed due to circular dependencies
                        // TODO: Consider using something more advanced such as cycle detection
                        _dirtyCallBlocks.ForEach(kvp => Enqueue(kvp.Key));
                        continue;
                    }
                }

                if (concurrent)
                {
                    Parallel.For(0, n, (i) => Process(todoBlocks[i]));
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        Process(todoBlocks[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Re-run the analysis for the specified methods. Repeatively propagate the changes of their return types
        /// to their callers, until there are none left.
        /// </summary>
        /// <remarks>
        /// It is expected that the introduced changes don't change the semantics of the program and hence don't
        /// increase the set of possible return types of the particular methods.
        /// </remarks>
        internal void Update(IEnumerable<SourceMethodSymbol> updatedMethods, bool concurrent = false)
        {
            // Initialize the currently re-analysed set of methods with the given ones
            _currentMethodsLastReturnTypes = new Dictionary<SourceMethodSymbol, bool>();
            foreach (var method in updatedMethods)
            {
                _currentMethodsLastReturnTypes.Add(method, false);
            }

            do
            {
                foreach (var kvp in _currentMethodsLastReturnTypes)
                {
                    kvp.Key.ControlFlowGraph.FlowContext.InvalidateAnalysis();
                    Enqueue(kvp.Key.ControlFlowGraph.Start);
                }

                // Re-run the analysis with the invalidated method flow information
                DoAll(concurrent);

                var lastMethods = _currentMethodsLastReturnTypes;
                _currentMethodsLastReturnTypes = new Dictionary<SourceMethodSymbol, bool>();

                // Check the changes of the return types and enlist the callers for the next round
                foreach (var kvp in lastMethods)
                {
                    // if (kvp.Key.ResultTypeMask != kvp.Value)
                    // {
                    //     var callers = _callGraph.GetCallerEdges(kvp.Key)
                    //         .Select(e => e.Caller)
                    //         .Where(c => !_currentMethodsLastReturnTypes
                    //             .ContainsKey(c)); // These were already reanalysed in this phase
                    //
                    //     foreach (var caller in callers)
                    //     {
                    //         _currentMethodsLastReturnTypes.Add(caller, caller.ResultTypeMask);
                    //     }

                    // }
                }
            } while (_currentMethodsLastReturnTypes.Count > 0);

            _currentMethodsLastReturnTypes = null;
        }

        /// <summary>
        /// Fills the given array with dequeued blocks from <see cref="_queue"/>./
        /// </summary>
        int Dequeue(BoundBlock[] todoBlocks)
        {
            // Helper data structures to enable adding only one block per method to a batch
            var todoContexts = new HashSet<TypeRefContext>();
            List<BoundBlock> delayedBlocks = null;

            // Insert the blocks with the highest priority to the batch while having at most one block
            // from each method, delaying the rest
            int n = 0;
            while (n < todoBlocks.Length && _queue.TryDequeue(out BoundBlock block))
                //TODO: TryDequeue() with a predicate so we won't have to maintain {delayedBlocks}
            {
                // var typeCtx = block.FlowState?.FlowContext?.TypeRefContext;
                //
                // if (todoContexts.Add(typeCtx))
                // {
                //     todoBlocks[n++] = block;
                // }
                // else
                // {
                //     if (delayedBlocks == null)
                //     {
                //         delayedBlocks = new List<BoundBlock>();
                //     }
                //
                //     delayedBlocks.Add(block);
                // }
            }

            // Return the delayed blocks back to the queue to be deenqueued the next time
            if (delayedBlocks != null)
            {
                foreach (var block in delayedBlocks)
                {
                    _queue.Enqueue(block);
                }
            }

            return n;
        }

        sealed class BoundBlockComparer : IComparer<BoundBlock>
        {
            int IComparer<BoundBlock>.Compare(BoundBlock x, BoundBlock y)
            {
                // Each block must be inserted only once to a worklist
                Debug.Assert(!ReferenceEquals(x, y));

                return 0;
                
                // Sort the blocks via their topological order to minimize the analysis repetition
                //return x.Ordinal - y.Ordinal;
            }
        }
    }
}