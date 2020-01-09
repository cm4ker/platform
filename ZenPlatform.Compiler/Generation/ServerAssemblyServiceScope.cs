using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis.Diagnostics;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Compiler.Generation
{
    public class ServerAssemblyServiceScope
    {
        private readonly IAssemblyBuilder _builder;
        private SystemTypeBindings _sb;
        private const string _serviceInitializerNamespace = "Service";
        private const string _serviceInitializerName = "ServerInitializer";
        private const string _invokeServiceFieldName = "_is";

        private const string _servInitMethod = "Init";

        public ServerAssemblyServiceScope(IAssemblyBuilder builder)
        {
            _builder = builder;

            _sb = _builder.TypeSystem.GetSystemBindings();

            ServiceInitializerType = builder.DefineType(_serviceInitializerNamespace, _serviceInitializerName,
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AnsiClass
                | TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit,
                _sb.Object);

            ServiceInitializerType.AddInterfaceImplementation(_sb.ServerInitializer);

            ServiceInitializerInitMethod = ServiceInitializerType.DefineMethod(_servInitMethod, true, false, true);

            ServiceInitializerConstructor = ServiceInitializerType.DefineConstructor(false, _sb.InvokeService);

            InvokeServiceField =
                ServiceInitializerType.DefineField(_sb.InvokeService, _invokeServiceFieldName, false, false);

            var e = ServiceInitializerConstructor.Generator;

            e.LdArg_0()
                .EmitCall(_sb.Object.Constructors[0])
                .LdArg_0()
                .LdArg(1)
                .StFld(InvokeServiceField)
                .Ret();
        }

        public ITypeBuilder ServiceInitializerType { get; }

        public IMethodBuilder ServiceInitializerInitMethod { get; }

        public IConstructorBuilder ServiceInitializerConstructor { get; }

        public IField InvokeServiceField { get; private set; }

        public void EndBuild()
        {
            var e = ServiceInitializerInitMethod.Generator;
            e.Ret();
        }
    }


    public enum VarTreeLeafType
    {
        Root,
        None,
        Prop,
        Func
    }

    public class GlobalVarTreeItem : Node
    {
        private readonly Action<IEmitter> _e;
        private List<object> _args;
        private object _codeObject;
        private CompilationMode _mode;

        public GlobalVarTreeItem(VarTreeLeafType type, CompilationMode mode, string name, Action<IEmitter> e)
        {
            _e = e;
            Type = type;
            Name = name;
            _args = new List<object>();
        }

        public string Name { get; }

        public VarTreeLeafType Type { get; }

        public object CodeObject => _codeObject;

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

        public void Emit(IEmitter e)
        {
            _e(e);
        }

        private IReadOnlyList<object> Args => _args.AsReadOnly();
    }

    public class GlobalVarManager : IGlobalVarManager
    {
        public GlobalVarManager(CompilationMode mode)
        {
            Root = new GlobalVarTreeItem(VarTreeLeafType.Root, CompilationMode.Shared, "NoName", null);
        }

        private GlobalVarTreeItem Root { get; }

        public void Register(GlobalVarTreeItem node)
        {
            node.Attach(Root);
        }

        public void Register(Node node)
        {
            if (!(node is GlobalVarTreeItem gvar))
                throw new Exception("Only GlobalVarTreeItem can be in GlobalVarTree");

            Register(gvar);
        }

        public void Emit(IEmitter e, GlobalVar exp, Action<object> onUnknown)
        {
            EmitInternal(e, exp.Expression, Root, onUnknown);
        }

        private void EmitInternal(IEmitter e, Expression exp, GlobalVarTreeItem currentItem,
            Action<object> onUnknown)
        {
            if (exp is LookupExpression le)
            {
                if (le.Lookup is Call c)
                {
                    var node = currentItem.Childs.Select(x => x as GlobalVarTreeItem)
                                   .FirstOrDefault(x => x.Name == c.Name && x.Type == VarTreeLeafType.Func) ??
                               throw new Exception(
                                   $"Node with name {c.Name} not found in global var. Component must register this name.");

                    onUnknown(c.Arguments);

                    onUnknown(c.Expression);

                    e.EmitCall((IMethod) node.CodeObject);
                }
                else if (le.Lookup is Name fe)
                {
                }
            }
        }
    }
}