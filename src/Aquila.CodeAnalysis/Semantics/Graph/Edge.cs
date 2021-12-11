﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.Syntax;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace Aquila.CodeAnalysis.Semantics.Graph
{
    public static class EdgeHelper
    {
        public static void NextCondition(this BoundBlock source, BoundBlock @true, BoundBlock @false,
            BoundExpression cond, bool isLoop = false)
        {
            Debug.Assert(source != null);

            var edge = new ConditionalEdge(@true, @false, cond) { IsLoop = isLoop };
            source.SetNextEdge(edge);
        }


        public static void NextSimple(this BoundBlock source, BoundBlock target)
        {
            Debug.Assert(source != target);

            var edge = new SimpleEdge(target);
            source.SetNextEdge(edge);
        }

        public static void NextLeave(this BoundBlock source, BoundBlock target)
        {
            Debug.Assert(source != target);

            var edge = new LeaveEdge(target);
            source.SetNextEdge(edge);
        }

        public static void NextTryCatch(this BoundBlock source, BoundBlock body, ImmutableArray<CatchBlock> catchBlocks,
            BoundBlock finallyBlock, BoundBlock endBlock)
        {
            var edge = new TryCatchEdge(body, catchBlocks, finallyBlock, endBlock);
            source.SetNextEdge(edge);
        }
    }

    /// <summary>
    /// Represents an edge to other blocks.
    /// </summary>
    public abstract partial class Edge
    {
        /// <summary>
        /// Associated syntax node.
        /// </summary>
        internal AquilaSyntaxNode AquilaSyntax { get; set; }

        /// <summary>
        /// Target blocks.
        /// </summary>
        public abstract IEnumerable<BoundBlock> Targets { get; }

        /// <summary>
        /// The block after the edge. Can be a <c>null</c> reference.
        /// </summary>
        public abstract BoundBlock NextBlock { get; }

        /// <summary>
        /// Gets value indicating whether the edge represents a conditional edge.
        /// </summary>
        public virtual bool IsConditional => false;

        /// <summary>
        /// Gets value indicating whether the edge represents try/catch.
        /// </summary>
        public virtual bool IsTryCatch => false;

        /// <summary>
        /// Gets value indicating whether the edge represents switch.
        /// </summary>
        public virtual bool IsSwitch => false;

        /// <summary>
        /// Condition expression of conditional edge.
        /// </summary>
        public virtual BoundExpression Condition => null;

        /// <summary>
        /// Catch blocks if try/catch edge.
        /// </summary>
        public virtual ImmutableArray<CatchBlock> CatchBlocks => ImmutableArray<CatchBlock>.Empty;

        /// <summary>
        /// Finally block of try/catch edge.
        /// </summary>
        public virtual BoundBlock FinallyBlock => null;

        /// <summary>
        /// Enumeration with single case blocks.
        /// </summary>
        public virtual ImmutableArray<MatchArmBlock> MatchBlocks => ImmutableArray<MatchArmBlock>.Empty;

        internal Edge()
        {
        }

        protected void Connect(BoundBlock source)
        {
            source.SetNextEdge(this);
        }

        /// <summary>
        /// Visits the object by given visitor.
        /// </summary>
        public abstract TResult Accept<TResult>(GraphVisitor<TResult> visitor);
    }

    /// <summary>
    /// Represents simple unconditional jump.
    /// </summary>
    [DebuggerDisplay("SimpleEdge")]
    public partial class SimpleEdge : Edge
    {
        /// <summary>
        /// Target block.
        /// </summary>
        public BoundBlock Target => _target;

        private readonly BoundBlock _target;

        /// <summary>
        /// Gets the target block if the simple edge.
        /// </summary>
        public override BoundBlock NextBlock => _target;

        internal SimpleEdge(BoundBlock target)
        {
            _target = target;
        }

        public SimpleEdge Update(BoundBlock target)
        {
            if (target == _target)
            {
                return this;
            }
            else
            {
                return new SimpleEdge(target);
            }
        }

        /// <summary>
        /// Target blocks.
        /// </summary>
        public override IEnumerable<BoundBlock> Targets => new BoundBlock[] { _target };

        /// <summary>
        /// Visits the object by given visitor.
        /// </summary>
        public override TResult Accept<TResult>(GraphVisitor<TResult> visitor) => visitor.VisitCFGSimpleEdge(this);
    }

    /// <summary>
    /// Represents an edge leaving try/catch block.
    /// The edge is not emitted.
    /// </summary>
    public partial class LeaveEdge : SimpleEdge
    {
        internal LeaveEdge(BoundBlock target)
            : base(target)
        {
        }

        public new LeaveEdge Update(BoundBlock target)
        {
            if (target == Target)
            {
                return this;
            }
            else
            {
                return new LeaveEdge(target);
            }
        }

        public override TResult Accept<TResult>(GraphVisitor<TResult> visitor) => visitor.VisitCFGLeaveEdge(this);
    }

    /// <summary>
    /// Conditional edge.
    /// </summary>
    [DebuggerDisplay("ConditionalEdge")]
    public sealed partial class ConditionalEdge : Edge
    {
        private readonly BoundBlock _true, _false;
        private readonly BoundExpression _condition;

        public override BoundBlock NextBlock => _false;

        /// <summary>
        /// Gets a value indicating the condition within a loop construct.
        /// </summary>
        public bool IsLoop { get; internal set; }

        /// <summary>
        /// Target true block.
        /// </summary>
        public BoundBlock TrueTarget => _true;

        /// <summary>
        /// Target false block.
        /// </summary>
        public BoundBlock FalseTarget => _false;

        internal ConditionalEdge(BoundBlock @true, BoundBlock @false, BoundExpression cond)
        {
            _true = @true;
            _false = @false;
            _condition = cond;
        }

        public ConditionalEdge Update(BoundBlock @true, BoundBlock @false, BoundExpression cond)
        {
            if (@true == _true && @false == _false && cond == _condition)
            {
                return this;
            }
            else
            {
                return new ConditionalEdge(@true, @false, cond);
            }
        }

        /// <summary>
        /// All target blocks.
        /// </summary>
        public override IEnumerable<BoundBlock> Targets => new BoundBlock[] { _true, _false };

        public override bool IsConditional
        {
            get { return true; }
        }

        public override BoundExpression Condition => _condition;

        /// <summary>
        /// Visits the object by given visitor.
        /// </summary>
        public override TResult Accept<TResult>(GraphVisitor<TResult> visitor) => visitor.VisitCFGConditionalEdge(this);
    }

    /// <summary>
    /// Represents try/catch/finally edge.
    /// </summary>
    [DebuggerDisplay("TryCatchEdge")]
    public sealed partial class TryCatchEdge : Edge
    {
        private readonly BoundBlock _body, _end;
        private readonly ImmutableArray<CatchBlock> _catchBlocks;
        private readonly BoundBlock _finallyBlock;

        /// <summary>
        /// Where <see cref="BodyBlock"/>, catch blocks and <see cref="FinallyBlock"/> go to.
        /// </summary>
        public override BoundBlock NextBlock => _end;

        /// <summary>
        /// Try block.
        /// </summary>
        public BoundBlock BodyBlock => _body;

        internal TryCatchEdge(BoundBlock body, ImmutableArray<CatchBlock> catchBlocks,
            BoundBlock finallyBlock, BoundBlock endBlock)
            : base()
        {
            Debug.Assert(catchBlocks != null);
            Debug.Assert(catchBlocks.Length != 0 || finallyBlock != null); // catch or finally or both

            _body = body;
            _catchBlocks = catchBlocks;
            _finallyBlock = finallyBlock;
            _end = endBlock;
        }

        public TryCatchEdge Update(BoundBlock body, ImmutableArray<CatchBlock> catchBlocks, BoundBlock finallyBlock,
            BoundBlock endBlock)
        {
            if (body == _body && catchBlocks == _catchBlocks && finallyBlock == _finallyBlock && endBlock == _end)
            {
                return this;
            }
            else
            {
                return new TryCatchEdge(body, catchBlocks, finallyBlock, endBlock)
                {
                };
            }
        }

        /// <summary>
        /// All target blocks.
        /// </summary>
        public override IEnumerable<BoundBlock> Targets
        {
            get
            {
                var size = _catchBlocks.Length + (_finallyBlock != null ? 2 : 1);
                var arr = new BoundBlock[size];

                // [ body, ...catches, finally]

                arr[0] = _body;
                ImmutableArray<BoundBlock>.CastUp(_catchBlocks).CopyTo(arr, 1);
                if (_finallyBlock != null)
                {
                    arr[size - 1] = _finallyBlock;
                }

                //
                return arr;
            }
        }

        public override bool IsTryCatch => true;

        public override ImmutableArray<CatchBlock> CatchBlocks => _catchBlocks;

        public override BoundBlock FinallyBlock => _finallyBlock;

        /// <summary>
        /// Ordinal of the block after the <c>try</c> scope.
        /// </summary>
        public int TryBlockScopeEnd => _catchBlocks.Length != 0 ? _catchBlocks[0].Ordinal : _finallyBlock.Ordinal;

        /// <summary>
        /// Visits the object by given visitor.
        /// </summary>
        public override TResult Accept<TResult>(GraphVisitor<TResult> visitor) => visitor.VisitCFGTryCatchEdge(this);
    }

    /// <summary>
    /// Represents foreach edge through the enumeree invocation.
    /// </summary>
    [DebuggerDisplay("ForeachEnumeree")]
    public sealed partial class ForeachEnumereeEdge : SimpleEdge
    {
        /// <summary>
        /// Array to enumerate through.
        /// </summary>
        public BoundExpression Enumeree => _enumeree;

        private readonly BoundExpression _enumeree;

        public bool AreValuesAliased => _aliasedValues;
        readonly bool _aliasedValues;

        internal ForeachEnumereeEdge(BoundBlock target, BoundExpression enumeree, bool aliasedValues)
            : base(target)
        {
            Contract.ThrowIfNull(enumeree);
            _enumeree = enumeree;
            _aliasedValues = aliasedValues;
        }

        public ForeachEnumereeEdge Update(BoundBlock target, BoundExpression enumeree, bool aliasedValues)
        {
            if (target == Target && enumeree == _enumeree && aliasedValues == _aliasedValues)
            {
                return this;
            }
            else
            {
                return new ForeachEnumereeEdge(target, enumeree, aliasedValues);
            }
        }

        /// <summary>
        /// Visits the object by given visitor.
        /// </summary>
        public override TResult Accept<TResult>(GraphVisitor<TResult> visitor) =>
            visitor.VisitCFGForeachEnumereeEdge(this);
    }

    /// <summary>
    /// Represents foreach edge from enumeree invocation through <c>MoveNext</c> to body block or end.
    /// </summary>
    [DebuggerDisplay("ForeachMoveNextEdge")]
    public sealed partial class ForeachMoveNextEdge : Edge
    {
        readonly BoundBlock _end;

        /// <summary>
        /// Span of the move expression to emit sequence point of <c>MoveNext</c> operation.
        /// </summary>
        public TextSpan MoveNextSpan { get; }

        /// <summary>
        /// Content of the foreach.
        /// </summary>
        public BoundBlock BodyBlock { get; }

        /// <summary>
        /// Block after the foreach.
        /// </summary>
        public override BoundBlock NextBlock => _end;

        /// <summary>
        /// Reference to the edge defining the enumeree.
        /// </summary>
        public ForeachEnumereeEdge EnumereeEdge { get; }

        /// <summary>
        /// Variable to store key in (can be null).
        /// </summary>
        public BoundReferenceEx KeyVariable { get; }

        /// <summary>
        /// Variable to store value in
        /// </summary>
        public BoundReferenceEx ValueVariable { get; }

        internal ForeachMoveNextEdge(BoundBlock body, BoundBlock end, ForeachEnumereeEdge enumereeEdge,
            BoundReferenceEx keyVar, BoundReferenceEx valueVar, TextSpan moveSpan)
        {
            Contract.ThrowIfNull(body);
            Contract.ThrowIfNull(end);
            Contract.ThrowIfNull(enumereeEdge);

            this.BodyBlock = body;
            _end = end;
            this.EnumereeEdge = enumereeEdge;
            this.KeyVariable = keyVar;
            this.ValueVariable = valueVar;
            this.MoveNextSpan = moveSpan;
        }

        public ForeachMoveNextEdge Update(BoundBlock body, BoundBlock end, ForeachEnumereeEdge enumereeEdge,
            BoundReferenceEx keyVar, BoundReferenceEx valueVar, TextSpan moveSpan)
        {
            if (body == BodyBlock && end == _end && enumereeEdge == EnumereeEdge && keyVar == KeyVariable &&
                valueVar == ValueVariable && moveSpan == MoveNextSpan)
            {
                return this;
            }
            else
            {
                return new ForeachMoveNextEdge(body, end, enumereeEdge, keyVar, valueVar, moveSpan);
            }
        }

        public override IEnumerable<BoundBlock> Targets
        {
            get { return new BoundBlock[] { BodyBlock, _end }; }
        }

        /// <summary>
        /// Visits the object by given visitor.
        /// </summary>
        public override TResult Accept<TResult>(GraphVisitor<TResult> visitor) =>
            visitor.VisitCFGForeachMoveNextEdge(this);
    }

    /// <summary>
    /// Represents switch edge.
    /// </summary>
    [DebuggerDisplay("SwitchEdge")]
    public sealed partial class MatchEdge : Edge
    {
        readonly BoundExpression _switchValue;
        readonly ImmutableArray<MatchArmBlock> _matchBlocks;

        readonly BoundBlock _end;

        public override BoundBlock NextBlock => _end;

        /// <summary>
        /// The expression representing the switch value.
        /// </summary>
        public BoundExpression SwitchValue => _switchValue;

        public override IEnumerable<BoundBlock> Targets => _matchBlocks;

        public override bool IsSwitch => true;

        public override ImmutableArray<MatchArmBlock> MatchBlocks => _matchBlocks;

        /// <summary>
        /// Gets the case blocks representing a default section.
        /// </summary>
        public MatchArmBlock DefaultBlock => _matchBlocks.FirstOrDefault(c => c.IsDefault);

        internal MatchEdge(BoundExpression switchValue, ImmutableArray<MatchArmBlock> matchBlocks, BoundBlock endBlock)
        {
            Contract.ThrowIfDefault(matchBlocks);
            _switchValue = switchValue;
            _matchBlocks = matchBlocks;
            _end = endBlock;
        }

        public MatchEdge Update(BoundExpression switchValue, ImmutableArray<MatchArmBlock> caseBlocks,
            BoundBlock endBlock)
        {
            if (switchValue == _switchValue && caseBlocks == _matchBlocks && endBlock == _end)
            {
                return this;
            }
            else
            {
                return new MatchEdge(switchValue, caseBlocks, endBlock);
            }
        }

        /// <summary>
        /// Visits the object by given visitor.
        /// </summary>
        public override TResult Accept<TResult>(GraphVisitor<TResult> visitor) => visitor.VisitCFGSwitchEdge(this);
    }
}