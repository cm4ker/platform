using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Описывает тело типа (методы, поля, события, конструкторы и т.д.)
    /// </summary>
    public partial class TypeBody
    {
        public TypeBody(IList<Member> members) : this(null,
            members.Where(x => x is Function).Cast<Function>().ToList(),
            members.Where(x => x is Field).Cast<Field>().ToList(),
            members.Where(x => x is Property).Cast<Property>().ToList(),
            members.Where(x => x is Constructor).Cast<Constructor>().ToList())
        {
        }


        public void AddFunction(Function function)
        {
            if (function == null) throw new ArgumentNullException(nameof(function));
            
            Functions.Add(function);
            Childs.Add(function);
        }

        public void AddConstructor(Constructor constructor)
        {
            if (constructor == null) throw new ArgumentNullException(nameof(constructor));
            
            Constructors.Add(constructor);
            Childs.Add(constructor);
        }
    }
}