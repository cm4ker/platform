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
        public static TypeBody Empty => new TypeBody(new List<Member>(), new UsingList());

        public TypeBody(IList<Member> members, UsingList usings) : this(null,
            members.Where(x => x is Function).Cast<Function>().ToAstList<FunctionList, Function>(),
            members.Where(x => x is Field).Cast<Field>().ToAstList<FieldList, Field>(),
            members.Where(x => x is Property).Cast<Property>().ToAstList<PropertyList, Property>(),
            members.Where(x => x is Constructor).Cast<Constructor>().ToAstList<ConstructorList, Constructor>(), usings)
        {
        }
    }
}