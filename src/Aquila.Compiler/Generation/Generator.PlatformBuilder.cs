using System.Collections.Generic;
using System.Linq;
using Portable.Xaml;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Visitor;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Data;
using Aquila.Language.Ast.Definitions;
using Aquila.ServerRuntime;

namespace Aquila.Compiler.Generation
{
    public partial class Generator
    {
        public void BuildConf()
        {
            _root = new Root(null, new CompilationUnitList());

            foreach (var component in _conf.TypeManager.Components)
            {
                if (component.TryGetFeature<IBuildingParticipant>(out var comBuild))

                    foreach (var type in _conf.TypeManager.Types.Where(x =>
                        x.ComponentId == component.Id && x.IsAsmAvaliable))
                    {
                        if (_mode == CompilationMode.Client)
                            comBuild.Generator.StageClient(type, _root);
                        else
                            comBuild.Generator.StageServer(type, _root);
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
                if (component.TryGetFeature<IBuildingParticipant>(out var bp))
                    foreach (var type in _conf.TypeManager.Types.Where(x =>
                        x.ComponentId == component.Id && x.IsAsmAvaliable))
                    {
                        if (_mode == CompilationMode.Client)
                            bp.Generator.StageClient(type, root);
                        else
                            bp.Generator.StageServer(type, root);
                    }
            }

            _cus = root.Units;
            // AstPlatformTypes.System(_root, _asm.TypeSystem);
            AstScopeRegister.Apply(root);

            return root;
        }
    }
}