using System.Collections;
using System.Collections.Generic;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Definitions.Statements;

namespace Aquila.Language.Ast.Infrastructure
{
    public class SyntaxCollection<T> : IEnumerable<T> where T : SyntaxNode
    {
        private readonly SyntaxNode _owner;
        private List<T> _collection;

        private int _currSlot;


        public SyntaxCollection(SyntaxNode owner, int startSlotPlacement)
        {
            _owner = owner;
            _collection = new List<T>();
            _currSlot = startSlotPlacement;
        }

        public T this[int value] => _collection[value];

        public void Add(T element)
        {
            _collection.Add(element);
            //_owner.Childs.SetSlot(element, _currSlot);
            _currSlot++;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _collection.Count;
    }

    public class ElementCollection : List<Element>
    {
    }

    public class FunctionCollection : List<Function>
    {
    }

    public class VariableCollection : List<Variable>
    {
    }

    public class ParameterCollection : List<Parameter>
    {
    }

    public class AttributeCollection : List<AttributeSyntax>
    {
    }

    public class MemberCollection : List<Member>
    {
    }

    public class StatementCollection : List<Statement>
    {
    }

    public class ArgumentCollection : List<Argument>
    {
    }

    public class FieldCollection : List<Field>
    {
    }

    public class PropertyCollection : List<Property>
    {
    }

    public class TypeCollection : List<TypeSyntax>
    {
    }
}