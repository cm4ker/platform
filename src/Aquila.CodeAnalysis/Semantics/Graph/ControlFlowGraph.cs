using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis.Semantics.Graph
{
    /// <summary>
    /// Represents statements control flow graph.
    /// </summary>
    public sealed partial class ControlFlowGraph
    {
        #region LabelBlockFlags, LabelBlockInfo

        /// <summary>
        /// Found label reference (definition or target) information.
        /// </summary>
        [Flags]
        public enum LabelBlockFlags : byte
        {
            /// <summary>
            /// Not used nor defined.
            /// </summary>
            None = 0,

            /// <summary>
            /// Label is defined.
            /// </summary>
            Defined = 1,

            /// <summary>
            /// Label is used as a target.
            /// </summary>
            Used = 2,

            /// <summary>
            /// Label was defined twice or more.
            /// </summary>
            Redefined = 4,
        }

        /// <summary>
        /// Label state.
        /// </summary>
        public sealed class LabelBlockState
        {
            /// <summary>
            /// Label identifier.
            /// </summary>
            public string Label;

            /// <summary>
            /// Positions of label definition and/or last label use.
            /// </summary>
            public TextSpan LabelSpan;

            /// <summary>
            /// Lable target block.
            /// </summary>
            public BoundBlock TargetBlock;

            /// <summary>
            /// Label information.
            /// </summary>
            public LabelBlockFlags Flags;
        }

        #endregion

        #region Fields & Properties

        /// <summary>
        /// Gets the control flow start block. Cannot be <c>null</c>.
        /// </summary>
        public BoundBlock Start
        {
            get { return _start; }
        }

        readonly BoundBlock
            _start;

        /// <summary>
        /// Gets the control flow exit block. Cannot be <c>null</c>.
        /// </summary>
        public BoundBlock Exit
        {
            get { return _exit; }
        }

        readonly BoundBlock
            _exit;

        ///// <summary>
        ///// Exception block. Can be <c>null</c>.
        ///// If set, code can throw an exception or be terminated by call to <c>exit</c>, before reaching exit block.
        ///// This block is connected with blocks ending with <c>throw</c> statement.
        ///// </summary>
        //public BoundBlock Throws { get { return _exception; } }
        //readonly BoundBlock _exception;

        /// <summary>
        /// Array of labels within method. Can be <c>null</c>.
        /// </summary>
        public ImmutableArray<LabelBlockState> Labels
        {
            get { return _labels; }
        }

        readonly ImmutableArray<LabelBlockState> _labels;

        /// <summary>
        /// Array of yield statements within method. Can be <c>null</c>.
        /// </summary>
        public ImmutableArray<BoundYieldStmt> Yields
        {
            get => _yields;
        }

        readonly ImmutableArray<BoundYieldStmt> _yields;

        /// <summary>
        /// List of blocks that are unreachable syntactically (statements after JumpStmt etc.).
        /// </summary>
        public ImmutableArray<BoundBlock> UnreachableBlocks
        {
            get { return _unreachable; }
        }

        readonly ImmutableArray<BoundBlock>
            _unreachable;

        /// <summary>
        /// Last "tag" color used. Used internally for graph algorithms.
        /// </summary>
        int _lastcolor = 0;

        #endregion

        #region Construction

        internal ControlFlowGraph(IList<StmtSyntax> statements, Binder binder)
            : this(BuilderVisitor.Build(statements, binder))
        {
        }

        internal ControlFlowGraph(IReadOnlyList<HtmlNodeSyntax> nodes, Binder binder)
            : this(BuilderVisitor.Build(nodes, binder))
        {
        }
        
        private ControlFlowGraph(BuilderVisitor builder)
            : this(builder.Start, builder.Exit, builder.Declarations, /*builder.Exception*/null, builder.Labels,
                builder.DeadBlocks)
        {
        }

        private ControlFlowGraph(BoundBlock start, BoundBlock exit,
            IEnumerable<BoundStatement> declarations, BoundBlock exception, ImmutableArray<LabelBlockState> labels,
            ImmutableArray<BoundBlock> unreachable)
        {
            Contract.ThrowIfNull(start);
            Contract.ThrowIfNull(exit);

            _start = start;
            _exit = exit;
            _start.Statements.InsertRange(0, declarations);

            //_exception = exception;
            _labels = labels;
            _unreachable = unreachable;
        }

        internal ControlFlowGraph Update(BoundBlock start, BoundBlock exit, ImmutableArray<LabelBlockState> labels,
            ImmutableArray<BoundYieldStmt> yields, ImmutableArray<BoundBlock> unreachable)
        {
            if (start == _start && exit == _exit && labels == _labels && yields == _yields &&
                unreachable == _unreachable)
            {
                return this;
            }
            else
            {
                return new ControlFlowGraph(start, exit, ImmutableArray<BoundStatement>.Empty, null, labels,
                    unreachable)
                {
                    _lastcolor = this._lastcolor
                };
            }
        }

        #endregion

        /// <summary>
        /// Gets new (unique) color for use by graph algorithms.
        /// </summary>
        /// <returns>New color index.</returns>
        public int NewColor()
        {
            return unchecked(++_lastcolor);
        }

        /// <summary>
        /// Visits control flow blocks and contained statements, in deep.
        /// Unreachable blocks are not visited.
        /// </summary>
        /// <remarks>Visitor does not implement infinite recursion prevention.</remarks>
        public TResult Accept<TResult>(GraphVisitor<TResult> visitor) => visitor.VisitCFG(this);
    }
}