using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        public void BuildConf()
        {
            var root = new Root(null, new List<CompilationUnit>());

            foreach (var component in _conf.Data.Components)
            {
                foreach (var type in component.Types)
                {
                    if (_mode == CompilationMode.Client)
                        component.ComponentImpl.Generator.StageClient(type, root);
                    else
                        component.ComponentImpl.Generator.StageServer(type, root);
                }
            }

            _cus = root.Units;
            AstScopeRegister.Apply(root);

            Build();
        }
    }
}