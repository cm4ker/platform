using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public class RBlockBuilder
    {
        private readonly RoslynInvokableBase _method;
        private RBlockBuilder _parent;

        private int _localIndex = 0;

        private List<RLocal> _locals = new List<RLocal>();

        Stack<object> _stack = new Stack<object>();

        private int GetNextLocIndex()
        {
            if (_parent is null)
            {
                return _localIndex++;
            }
            else return _parent.GetNextLocIndex();
        }


        public List<Expression> BaseCall { get; set; } = new List<Expression>();

        public RBlockBuilder(RoslynTypeSystem ts, RoslynInvokableBase method) : this(ts, method, null)
        {
        }

        public RBlockBuilder(RoslynTypeSystem ts, RoslynInvokableBase method, RBlockBuilder parent)
        {
            _parent = parent;
            TypeSystem = ts;
            _method = method;
        }

        public RoslynTypeSystem TypeSystem { get; }

        private Expression PopExp()
        {
            return (Expression) Pop();
        }

        private RBlockBuilder PopBlock()
        {
            return (RBlockBuilder) Pop();
        }

        private RoslynType PopType()
        {
            return (RoslynType) Pop();
        }

        public RLocal DefineLocal(RoslynType type)
        {
            var loc = new RLocal($"loc{GetNextLocIndex()}", type);
            _locals.Add(loc);
            return loc;
        }

        public RBlockBuilder Push(object exp)
        {
            _stack.Push(exp);
            return this;
        }

        public RBlockBuilder StLoc(RLocal loc)
        {
            _stack.Push(new Assign(PopExp(), new NameExpression(loc.Name)));
            return this;
        }

        public RBlockBuilder StFld(RoslynField field)
        {
            _stack.Push(new Assign(PopExp(), new LookUp(new NameExpression(field.Name), PopExp())));
            return this;
        }

        public RBlockBuilder StSFld(RoslynField field)
        {
            _stack.Push(new Assign(PopExp(),
                new LookUp(new NameExpression(field.Name), new TypeToken(field.DeclaringType))));
            return this;
        }

        public RBlockBuilder LdSFld(RoslynField field)
        {
            _stack.Push(new LookUp(new NameExpression(field.Name), new TypeToken(field.DeclaringType)));
            return this;
        }

        public RBlockBuilder NewObj(RoslynConstructor c)
        {
            Expression[] args = new Expression[c.Parameters.Count];
            for (int i = c.Parameters.Count - 1; i >= 0; i--)
            {
                args[i] = PopExp();
            }

            Push(new NewObjectExpression(c, args));
            return this;
        }

        /*
         
         var i = new int[10];
         i[0] = 1;
         ..
         i[1] = 2;
         ..
         i[2]
         ....
         
         var i = new int[10] {1,2,3,4,5};
         
         new Call(new int[10] {1,2,3,4,5});
         
         */

        public RBlockBuilder NewArr(RoslynType c)
        {
            Push(new NewArrayExpression(c, PopExp()));
            return this;
        }

        public AdvancedArrayBuilder NewArrAdv(RoslynType c)
        {
            return new AdvancedArrayBuilder(PopExp(), c, this);
        }

        public RBlockBuilder LdLoc(RLocal loc)
        {
            _stack.Push(new NameExpression(loc.Name));
            return this;
        }

        public RBlockBuilder LdArg(int index)
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


        public RBlockBuilder LdFld(RoslynField fld)
        {
            _stack.Push(new LookUp(new NameExpression(fld.Name), PopExp()));
            return this;
        }

        public RBlockBuilder LdArg(RoslynParameter p)
        {
            _stack.Push(new NameExpression(p.Name));
            return this;
        }

        public RBlockBuilder LdArg_0()
        {
            return LdArg(0);
        }

        public RBlockBuilder LdLit(int i)
        {
            _stack.Push(new Literal(i));
            return this;
        }

        public RBlockBuilder LdLit(char i)
        {
            _stack.Push(new Literal(i));
            return this;
        }

        public RBlockBuilder LdLit(decimal d)
        {
            _stack.Push(new Literal(d));
            return this;
        }

        public RBlockBuilder LdLit(uint i)
        {
            _stack.Push(new Literal(i));
            return this;
        }

        public RBlockBuilder LdLit(string s)
        {
            _stack.Push(new Literal(s));
            return this;
        }

        public RBlockBuilder LdLit(double i)
        {
            _stack.Push(new Literal(i));
            return this;
        }


        public RBlockBuilder LdFtn(RoslynMethod method)
        {
            _stack.Push(new NameExpression(method.Name));
            return this;
        }

        public RBlockBuilder LdElem()
        {
            Push(new IndexerAccess(PopExp(), PopExp()));
            return this;
        }

        public RBlockBuilder StElem()
        {
            Push(new Assign(PopExp(), new IndexerAccess(PopExp(), PopExp())));
            return this;
        }

        public RBlockBuilder Dup()
        {
            var dup = Pop();
            Push(dup);
            Push(dup);

            return this;
        }

        public RBlockBuilder Add()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Plus));
            return this;
        }

        public RBlockBuilder Neg()
        {
            _stack.Push(new UnaryExpression(PopExp(), UKind.Negative));
            return this;
        }

        public RBlockBuilder Not()
        {
            _stack.Push(new UnaryExpression(PopExp(), UKind.Not));
            return this;
        }

        public RBlockBuilder Sub()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Minus));
            return this;
        }

        public RBlockBuilder Mul()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Multiply));
            return this;
        }

        public RBlockBuilder Div()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Div));
            return this;
        }

        public RBlockBuilder Rem()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Rem));
            return this;
        }

        public RBlockBuilder StArg(RoslynParameter arg)
        {
            _stack.Push(new Assign(PopExp(), new NameExpression(arg.Name)));
            return this;
        }

        public RBlockBuilder Cgt()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Gt));
            return this;
        }

        public RBlockBuilder Ceq()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Eq));
            return this;
        }

        public RBlockBuilder Clt()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Lt));
            return this;
        }

        public RBlockBuilder GreaterOrEqual()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Get));
            return this;
        }

        public RBlockBuilder LessOrEqual()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Let));
            return this;
        }

        public RBlockBuilder And()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.And));
            return this;
        }

        public RBlockBuilder Or()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Or));
            return this;
        }

        public RBlockBuilder Cneq()
        {
            _stack.Push(new BinaryExpression(PopExp(), PopExp(), BKind.Ne));
            return this;
        }

        public RBlockBuilder Nothing()
        {
            _stack.Push(null);
            return this;
        }

        public RBlockBuilder Declare()
        {
            _stack.Push(new Declare(PopExp(), (NameExpression) _stack.Pop(), PopType()));

            return this;
        }

        public RBlockBuilder Inline(RLocal loc)
        {
            _locals.Remove(loc);
            _stack.Push(new Declare(PopExp(), new NameExpression(loc.Name), loc.Type));
            return this;
        }

        public RBlockBuilder Assign()
        {
            _stack.Push(new Assign(PopExp(), PopExp()));
            return this;
        }

        public RBlockBuilder For()
        {
            _stack.Push(new For((RBlockBuilder) Pop(), PopExp(), PopExp(), PopExp()));
            return this;
        }

        public RBlockBuilder Block()
        {
            var b = new RBlockBuilder(TypeSystem, _method, this);
            Push(b);
            return b;
        }

        public RBlockBuilder EndBlock()
        {
            return _parent;
        }

        public RBlockBuilder If()
        {
            _stack.Push(new RXIf(PopBlock(), PopBlock(), PopExp()));
            return this;
        }

        public RBlockBuilder While()
        {
            _stack.Push(new RXWhile(PopBlock(), PopExp()));
            return this;
        }

        public RBlockBuilder Try()
        {
            _stack.Push(new RXTry(PopBlock(), PopBlock(), PopBlock()));
            return this;
        }


        public RBlockBuilder TryCall()
        {
            return this;
        }

        public RBlockBuilder Ret()
        {
            if (_method is RoslynConstructor ||
                _method is RoslynMethod m && m.ReturnType == m.System.GetSystemBindings().Void)
                Nothing();

            _stack.Push(new Return(PopExp()));
            return this;
        }

        public RBlockBuilder IsInst(RoslynType type)
        {
            _stack.Push(new Is(type, PopExp()));
            return this;
        }

        public RBlockBuilder Cast(RoslynType type)
        {
            _stack.Push(new Cast(type, PopExp()));
            return this;
        }

        public RBlockBuilder Throw()
        {
            Push(new RXThrow(PopExp()));
            return this;
        }

        public RBlockBuilder Throw(RoslynType type)
        {
            var con = type.Constructors.FirstOrDefault(x => !x.Parameters.Any());

            if (con == null)
                throw new Exception("Exception haven't default constructor use NewObj + Throw instead");

            NewObj(con);
            return Throw();
        }

        public RBlockBuilder StProp(RoslynProperty prop)
        {
            if (prop.Setter.IsStatic)
                throw new Exception("This not allowed yet");

            Push(new Assign(PopExp(), new LookUp(new NameExpression(prop.Name), PopExp())));

            return this;
        }

        public RBlockBuilder LdProp(RoslynProperty prop)
        {
            if (prop.Getter.IsStatic)
            {
                Push(new LookUp(new NameExpression(prop.Name), new TypeToken(prop.DeclaringType)));
            }
            else

                Push(new LookUp(new NameExpression(prop.Name), PopExp()));

            return this;
        }

        public RBlockBuilder Call(RoslynInvokableBase method)
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
                    try
                    {
                        var stmt = new Statement((Expression) item);
                        //((Statement) item).Dump(tw);
                        stmt.Dump(tw);
                        tw.W("\n");
                    }
                    catch (Exception ex)
                    {
                        if (item is Expression)
                            Console.WriteLine(
                                $"Founded unwrapped instruction {item} on:\nType: {_method.DeclaringType.Name}\nMethod: {_method.Name}");
                    }
                }
            }
        }

        public RBlockBuilder Null()
        {
            Push(new NameExpression("null"));
            return this;
        }

        public RBlockBuilder LdNull()
        {
            return Null();
        }
    }
}