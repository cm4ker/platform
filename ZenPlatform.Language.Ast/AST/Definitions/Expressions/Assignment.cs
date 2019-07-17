using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Expressions
{
    public class Assignment : Expression
    {
        private const int NAME_SLOT = 0;
        private const int INDEX_SLOT = 1;
        private const int VALUE_SLOT = 2;

        private Name _name;
        private Expression _value;
        private Expression _index;


        public Assignment(ILineInfo lineInfo, Expression value, Expression index, Name name) : base(lineInfo)
        {
            _name = Children.SetSlot(name, NAME_SLOT);
            _value = Children.SetSlot(index, VALUE_SLOT);
            _index = Children.SetSlot(value, INDEX_SLOT);
        }

        public Name Name
        {
            get => _name ?? Children.GetSlot(out _name, NAME_SLOT);
            set => Children.SetSlot(_name, NAME_SLOT);
        }

        public Expression Value
        {
            get => _value ?? Children.GetSlot(out _value, VALUE_SLOT);
            set => Children.SetSlot(_value, VALUE_SLOT);
        }

        public Expression Index
        {
            get => _index ?? Children.GetSlot(out _index, INDEX_SLOT);
            set => Children.SetSlot(_index, INDEX_SLOT);
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitAssigment(this);
        }
    }

    /// <summary>
    /// Выражение для постинкрементирования
    /// </summary>
    public class PostIncrementExpression : Expression
    {
        private const int NAME_SLOT = 0;

        private Name _name;

        public Name Name => _name ?? Children.GetSlot(out _name, NAME_SLOT);

        public PostIncrementExpression(ILineInfo li, string name) : this(li, new Name(li, name))
        {
        }

        public PostIncrementExpression(ILineInfo lineInfo, Name name) : base(lineInfo)
        {
            _name = Children.SetSlot(name, NAME_SLOT);
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPostIncrementStatement(this);
        }
    }

    public class PostDecrementExpression : Expression
    {
        private const int NAME_SLOT = 0;

        private Name _name;

        public Name Name => _name ?? Children.GetSlot(out _name, NAME_SLOT);

        public PostDecrementExpression(ILineInfo li, string name) : this(li, new Name(li, name))
        {
        }


        public PostDecrementExpression(ILineInfo lineInfo, Name name) : base(lineInfo)
        {
            _name = Children.SetSlot(name, NAME_SLOT);
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitPostDecrementStatement(this);
        }
    }
}