using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn.Operations;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public partial class RoslynEmitter
    {
        private readonly RoslynInvokableBase _method;
        private readonly RoslynEmitter _parent;

        private int _localIndex = 0;
        private int _labelIndex = 0;

        readonly List<RLocal> _locals = new List<RLocal>();
        readonly Stack<object> _stack = new Stack<object>();

        private int GetNextLocIndex()
        {
            if (_parent is null)
            {
                return _localIndex++;
            }
            else return _parent.GetNextLocIndex();
        }

        private int GetNextLabelIndex()
        {
            if (_parent is null)
            {
                return _labelIndex++;
            }
            else return _parent.GetNextLocIndex();
        }


        public List<Expression> BaseCall { get; set; } = new List<Expression>();

        public RoslynEmitter(RoslynTypeSystem ts, RoslynInvokableBase method) : this(ts, method, null)
        {
        }

        public RoslynEmitter(ITypeSystem ts, RoslynInvokableBase method, RoslynEmitter parent)
        {
            _parent = parent;
            TypeSystem = ts;
            _method = method;
        }

        public ITypeSystem TypeSystem { get; }

        private Expression PopExp()
        {
            return (Expression) Pop();
        }

        private RoslynEmitter PopBlock()
        {
            return (RoslynEmitter) Pop();
        }

        private RoslynType PopType()
        {
            return (RoslynType) Pop();
        }

        public ILocal DefineLocalInternal(IType type)
        {
            var loc = new RLocal(GetNextLocIndex(), type);
            _locals.Add(loc);
            return loc;
        }

        public RoslynEmitter Push(object exp)
        {
            _stack.Push(exp);
            return this;
        }

        public RoslynEmitter StLoc(RLocal loc)
        {
            _stack.Push(new Assign(PopExp(), new NameExpression(loc.Name)));
            return this;
        }

        public RoslynEmitter StFld(IField field)
        {
            _stack.Push(new Assign(PopExp(), new LookUp(new NameExpression(field.Name), PopExp())));
            return this;
        }

        public RoslynEmitter StSFld(IField field)
        {
            _stack.Push(new Assign(PopExp(),
                new LookUp(new NameExpression(field.Name), new TypeToken(field.DeclaringType))));
            return this;
        }

        public RoslynEmitter LdSFld(IField field)
        {
            _stack.Push(new LookUp(new NameExpression(field.Name), new TypeToken(field.DeclaringType)));
            return this;
        }

        public RoslynEmitter NewObj(IConstructor c)
        {
            Expression[] args = new Expression[c.Parameters.Count];
            for (int i = c.Parameters.Count - 1; i >= 0; i--)
            {
                args[i] = PopExp();
            }

            Push(new NewObjectExpression(c, args));
            return this;
        }

        public RoslynEmitter NewArr(IType c)
        {
            Push(new NewArrayExpression(c, PopExp()));
            return this;
        }

        public AdvancedArrayBuilder NewArrAdv(RoslynType c)
        {
            return new AdvancedArrayBuilder(PopExp(), c, this);
        }

        public RoslynEmitter LdLoc(RLocal loc)
        {
            _stack.Push(new NameExpression(loc.Name));
            return this;
        }

        public RoslynEmitter LdArg(int index)
        {
            if (!_method.IsStatic && index == 0)
            {
                _stack.Push(new NameExpression("this"));
            }
            else
            {
                var param = _method.Parameters.FirstOrDefault(x => x.ArgIndex == index) ??
                            throw new Exception(
                                $"Type: {_method.DeclaringType.FullName}\nParameter: {_method.Name}\nParameter with index {index} not found");
                _stack.Push(new NameExpression(param.Name));
            }

            return this;
        }

        public RoslynEmitter LdFld(RoslynField fld)
        {
            _stack.Push(new LookUp(new NameExpression(fld.Name), PopExp()));
            return this;
        }

        public RoslynEmitter LdArg(RoslynParameter p)
        {
            _stack.Push(new NameExpression(p.Name));
            return this;
        }

        public RoslynEmitter LdArg_0()
        {
            return LdArg(0);
        }

        public RoslynEmitter LdLit(int i)
        {
            _stack.Push(new Literal(i));
            return this;
        }

        public RoslynEmitter LdLit(char i)
        {
            _stack.Push(new Literal(i));
            return this;
        }

        public RoslynEmitter LdLit(decimal d)
        {
            _stack.Push(new Literal(d));
            return this;
        }

        public RoslynEmitter LdLit(long d)
        {
            _stack.Push(new Literal(d));
            return this;
        }

        public RoslynEmitter LdLit(uint i)
        {
            _stack.Push(new Literal(i));
            return this;
        }

        public RoslynEmitter LdLit(string s)
        {
            _stack.Push(new Literal(s));
            return this;
        }

        public RoslynEmitter LdLit(double i)
        {
            _stack.Push(new Literal(i));
            return this;
        }


        public RoslynEmitter LdFtn(RoslynMethod method)
        {
            _stack.Push(new NameExpression(method.Name));
            return this;
        }

        public RoslynEmitter LdElem()
        {
            Push(new IndexerAccess(PopExp(), PopExp()));
            return this;
        }

        public RoslynEmitter StElem()
        {
            Push(new Assign(PopExp(), new IndexerAccess(PopExp(), PopExp())));
            return this;
        }

        public RoslynEmitter Dup()
        {
            var dup = Pop();
            Push(dup);
            Push(dup);

            return this;
        }

        public RoslynEmitter Add()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Plus));
            return this;
        }

        public RoslynEmitter Neg()
        {
            _stack.Push(new UnaryExpression(PopExp(), UKind.Negative));
            return this;
        }


        internal RoslynEmitter Goto(ILabel label)
        {
            _stack.Push(new GoTo((RLabel) label));
            return this;
        }

        public RoslynEmitter Not()
        {
            _stack.Push(new UnaryExpression(PopExp(), UKind.Not));
            return this;
        }

        public RoslynEmitter Sub()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Minus));
            return this;
        }

        public RoslynEmitter Mul()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Multiply));
            return this;
        }

        public RoslynEmitter Div()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Div));
            return this;
        }

        public RoslynEmitter Rem()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Rem));
            return this;
        }

        public RoslynEmitter StArg(RoslynParameter arg)
        {
            _stack.Push(new Assign(PopExp(), new NameExpression(arg.Name)));
            return this;
        }

        public RoslynEmitter Cgt()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Gt));
            return this;
        }

        public RoslynEmitter Ceq()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Eq));
            return this;
        }

        public RoslynEmitter Clt()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Lt));
            return this;
        }

        public RoslynEmitter GreaterOrEqual()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Get));
            return this;
        }

        public RoslynEmitter LessOrEqual()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Let));
            return this;
        }

        public RoslynEmitter And()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.And));
            return this;
        }

        public RoslynEmitter Or()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Or));
            return this;
        }

        public RoslynEmitter Cneq()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Ne));
            return this;
        }

        public RoslynEmitter Nothing()
        {
            _stack.Push(null);
            return this;
        }

        public RoslynEmitter Declare()
        {
            _stack.Push(new Declare(PopExp(), (NameExpression) _stack.Pop(), PopType()));

            return this;
        }

        public RoslynEmitter Inline(RLocal loc)
        {
            _locals.Remove(loc);
            _stack.Push(new Declare(PopExp(), new NameExpression(loc.Name), loc.Type));
            return this;
        }

        public RoslynEmitter Assign()
        {
            _stack.Push(new Assign(PopExp(), PopExp()));
            return this;
        }

        public RoslynEmitter For()
        {
            _stack.Push(new RXFor((RoslynEmitter) Pop(), PopExp(), PopExp(), PopExp()));
            return this;
        }

        public RoslynEmitter Block()
        {
            var b = new RoslynEmitter(TypeSystem, _method, this);
            Push(b);
            return b;
        }

        public RoslynEmitter EndBlock()
        {
            return _parent;
        }

        public RoslynEmitter If()
        {
            _stack.Push(new RXIf(PopBlock(), PopBlock(), PopExp()));
            return this;
        }

        public RoslynEmitter While()
        {
            _stack.Push(new RXWhile(PopBlock(), PopExp()));
            return this;
        }

        public RoslynEmitter Try()
        {
            _stack.Push(new RXTry(PopBlock(), PopBlock(), PopBlock()));
            return this;
        }

        public RoslynEmitter Ret()
        {
            if (_method is RoslynConstructor ||
                _method is RoslynMethod m && m.ReturnType == m.System.GetSystemBindings().Void)
                Nothing();

            _stack.Push(new Return(PopExp()));
            return this;
        }

        public RoslynEmitter IsInst(IType type)
        {
            _stack.Push(new Is(type, PopExp()));
            return this;
        }

        public RoslynEmitter Cast(IType type)
        {
            _stack.Push(new Cast(type, PopExp()));
            return this;
        }

        public RoslynEmitter Throw()
        {
            Push(new RXThrow(PopExp()));
            return this;
        }

        public RoslynEmitter Mark(RLabel label)
        {
            Push(label);
            return this;
        }


        public RoslynEmitter Throw(RoslynType type)
        {
            var con = type.Constructors.FirstOrDefault(x => !x.Parameters.Any());

            if (con == null)
                throw new Exception("Exception haven't default constructor use NewObj + Throw instead");

            NewObj(con);
            return Throw();
        }

        public RoslynEmitter StProp(RoslynProperty prop)
        {
            if (prop.Setter.IsStatic)
                throw new Exception("This not allowed yet");

            Push(new Assign(PopExp(), new LookUp(new NameExpression(prop.Name), PopExp())));

            return this;
        }

        public RoslynEmitter LdProp(RoslynProperty prop)
        {
            if (prop.Getter.IsStatic)
            {
                Push(new LookUp(new NameExpression(prop.Name), new TypeToken(prop.DeclaringType)));
            }
            else

                Push(new LookUp(new NameExpression(prop.Name), PopExp()));

            return this;
        }

        public RoslynEmitter Call(RoslynInvokableBase method)
        {
            Expression[] args = new Expression[method.Parameters.Count];
            for (int i = method.Parameters.Count - 1; i >= 0; i--)
            {
                args[i] = PopExp();
            }

            if (_method is RoslynConstructor && method is RoslynConstructor)
            {
                //if we call constructor inside constructor then we want call base

                if (!method.IsStatic)
                {
                    PopExp();
                }

                if (BaseCall.Any())
                    throw new Exception("You call base constructor twice in the constructor. This should never happen");

                BaseCall.AddRange(args);
                return this;
            }

            if (method.IsStatic)
                Push(new Call(method, args));
            else

                Push(new LookUp(new Call(method, args), PopExp()));
            return this;
        }

        public object Pop()
        {
            if (_stack.Count == 0)
                throw new Exception("Stack underflow");
            return _stack.Pop();
        }

        public void Dump(TextWriter tw)
        {
            using (tw.CurlyBrace())
            {
                foreach (var loc in _locals)
                {
                    loc.Type.DumpRef(tw);
                    tw.Space().W(loc.Name).Comma().W("\n");
                }

                foreach (var item in _stack.Reverse())
                {
                    if (item is Expression exp)
                    {
                        var stmt = new Statement(exp);
                        stmt.Dump(tw);
                        tw.W("\n");
                    }
                    else if (item is RLabel label)
                    {
                        label.Dump(tw);
                        tw.W(":\n");
                    }
                }
            }
        }

        public RoslynEmitter Null()
        {
            Push(new NameExpression("null"));
            return this;
        }

        public RoslynEmitter LdNull()
        {
            return Null();
        }
    }
}