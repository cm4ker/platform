using System;
using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Compiler.Generation
{
    public class GlobalVarTreeItem : Node
    {
        private readonly Action<Node, RBlockBuilder> _e;
        private List<object> _args;
        private object _codeObject;
        private CompilationMode _mode;
        private TypeSyntax _astType;

        public GlobalVarTreeItem(VarTreeLeafType type, CompilationMode mode, string name, Action<Node, RBlockBuilder> e,
            TypeSyntax astType = null)
        {
            if (astType == null && type == VarTreeLeafType.Func)
                astType = new PrimitiveTypeSyntax(null, TypeNodeKind.Void);

            _astType = astType;

            _e = e;
            Type = type;
            Name = name;
            _args = new List<object>();
        }

        public string Name { get; }

        public VarTreeLeafType Type { get; }

        public object CodeObject => _codeObject;

        public TypeSyntax AstType => _astType;

        public void SetCodeObject(IField field)
        {
            _codeObject = field;
        }

        public void SetCodeObject(IMethod method)
        {
            _codeObject = method;
        }

        public void SetCodeObject(IProperty prop)
        {
            _codeObject = prop;
        }

        public void AddArgument(string arg)
        {
            _args.Add(arg);
        }

        public void AddArgument(int arg)
        {
            _args.Add(arg);
        }

        public void Emit(Node node, RBlockBuilder e)
        {
            _e(node, e);
        }

        private IReadOnlyList<object> Args => _args.AsReadOnly();
    }
}