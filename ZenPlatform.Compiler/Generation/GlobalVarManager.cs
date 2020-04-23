using System;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Extensions;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Compiler.Generation
{
    public class GlobalVarManager : IGlobalVarManager
    {
        public GlobalVarManager(CompilationMode mode, RoslynTypeSystem ts)
        {
            Root = new GlobalVarTreeItem(VarTreeLeafType.Root, CompilationMode.Shared, "NoName", null);
            TypeSystem = ts;
        }

        public Node Root { get; }

        public RoslynTypeSystem TypeSystem { get; }

        public void Register(Node node)
        {
            if (!(node is GlobalVarTreeItem gvar))
                throw new Exception("Only GlobalVarTreeItem can be in GlobalVarTree");

            Root.Attach(node);
        }

        public void Emit(IEmitter e, GlobalVar exp, Action<object> onUnknown)
        {
            EmitInternal(e, exp.Expression, Root, onUnknown);
        }

        private void EmitInternal(IEmitter e, Expression exp, Node currentItem,
            Action<object> onUnknown)
        {
            if (exp is LookupExpression le)
            {
                if (le.Lookup is Call c)
                {
                    var node = currentItem.Children.Select(x => x as GlobalVarTreeItem)
                                   .FirstOrDefault(x => x.Name == c.Name.Value && x.Type == VarTreeLeafType.Func) ??
                               throw new Exception(
                                   $"Node with name {c.Name} not found in global var. Component must register this name.");

                    onUnknown(c.Arguments);

                    //onUnknown(c.Expression);

                    e.EmitCall((IMethod) node.CodeObject);
                }
                else if (le.Lookup is Name fe)
                {
                }
            }
        }
    }
}