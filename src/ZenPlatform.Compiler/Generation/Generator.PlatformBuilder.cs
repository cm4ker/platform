using System.Collections.Generic;
using System.Linq;
using Portable.Xaml;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.ServerRuntime;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        public void BuildConf()
        {
            _root = new Root(null, new CompilationUnitList());

            foreach (var component in _conf.TypeManager.Components)
            {
                foreach (var type in _conf.TypeManager.Types.Where(x =>
                    x.ComponentId == component.Id && x.IsAsmAvaliable))
                {
                    if (_mode == CompilationMode.Client)
                        component.ComponentImpl.Generator.StageClient(type, _root);
                    else
                        component.ComponentImpl.Generator.StageServer(type, _root);
                }
            }

            _cus = _root.Units;
            AstScopeRegister.Apply(_root);

            //Add Querying support to the platform
            if (_mode.HasFlag(CompilationMode.Server))
                QueryCompilerHelper.Init(_root, _ts);

            Build();
        }

        public Root BuildAst()
        {
            var root = new Root(null, new CompilationUnitList());

            foreach (var component in _conf.TypeManager.Components)
            {
                foreach (var type in _conf.TypeManager.Types.Where(x =>
                    x.ComponentId == component.Id && x.IsAsmAvaliable))
                {
                    if (_mode == CompilationMode.Client)
                        component.ComponentImpl.Generator.StageClient(type, root);
                    else
                        component.ComponentImpl.Generator.StageServer(type, root);
                }
            }

            _cus = root.Units;
            // AstPlatformTypes.System(_root, _asm.TypeSystem);
            AstScopeRegister.Apply(root);

            return root;
        }
    }
}