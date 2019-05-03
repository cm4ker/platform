using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions.Functions
{
    /// <summary>
    /// Describes a parameter.
    /// </summary>
    public class Parameter : AstNode, ITypedNode
    {
        /// <summary>
        /// Parameter name.
        /// </summary>
        public string Name;

        /// <summary>
        /// Parameter type.
        /// </summary>
        public ZType Type { get; set; }

        /// <summary>
        /// Parameter pass method.
        /// </summary>
        public PassMethod PassMethod = PassMethod.ByValue;

        /// <summary>
        /// Create parameter object.
        /// </summary>
        public Parameter(string name, ZType type, PassMethod passMethod)
        {
            Name = name;
            Type = type;
            PassMethod = passMethod;
        }
    }

    public interface ITypedNode
    {
        ZType Type { get; set; }
    }
}