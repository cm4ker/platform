using System;
using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Олицетворяет класс в платформе
    /// </summary>
    public class Class : TypeEntity
    {
        /// <summary>
        /// Create a module object.
        /// </summary>
        public Class(ILineInfo li, TypeBody typeBody, string name) : base(li)
        {
            TypeBody = typeBody;
            Name = name;
        }


        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(TypeBody);
        }
    }
}