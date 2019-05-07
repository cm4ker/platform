using System.Threading;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Описывает тип
    /// </summary>
    public abstract class ZType : AstNode
    {
        public ZType(string name, string @namespace)
        {
            Name = name;
            Namespace = @namespace;
        }

        /// <summary>
        /// Имя типа
        /// </summary>
        public string Name { get; }

        public string Namespace { get; protected set; }

        public virtual bool IsArray { get; }
        public virtual bool IsSystem { get; }
    }

    public abstract class ZSystemType : ZType
    {
        public override bool IsSystem => true;

        public ZSystemType(string name) : base(name, "System")
        {
        }
    }

    public class ZInt : ZSystemType
    {
        public ZInt() : base("int")
        {
        }
    }

    public class ZDouble : ZSystemType
    {
        public ZDouble() : base("double")
        {
        }
    }

    public class ZBool : ZSystemType
    {
        public ZBool() : base("bool")
        {
        }
    }

    public class ZCharacter : ZSystemType
    {
        public ZCharacter() : base("char")
        {
        }
    }

    public class ZString : ZSystemType
    {
        public ZString() : base("string")
        {
        }
    }

    public class ZVoid : ZSystemType
    {
        public ZVoid() : base("void")
        {
        }
    }

    public class ZArray : ZType
    {
        public ZType TypeOfElements { get; }

        public ZArray(ZType typeOfElements) : base($"{typeOfElements.Name}[]", "System.Collections")
        {
            TypeOfElements = typeOfElements;
        }

        public override bool IsArray => true;
    }

    public class ZStructureType : ZType
    {
        public ZStructureType(string name) : base(name, null)
        {
        }

        public ZStructureType(string name, string @namespace) : base(name, @namespace)
        {
        }

        public void SetNamespace(string @namespace)
        {
            Namespace = @namespace;
        }
    }

    public class ZTypeSystem
    {
        public static ZType String = new ZString();
        public static ZType Double = new ZDouble();
        public static ZType Bool = new ZBool();
        public static ZType Int = new ZInt();
        public static ZType Char = new ZCharacter();
        public static ZType Void = new ZVoid();
    }
}