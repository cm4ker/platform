using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Language.Ast.AST.Builder;

namespace ZenPlatform.EntityComponent.Entity
{
    public class AstGenerator : IAstGenerator
    {
        private readonly XCComponent _component;
        private AstBuilder _builder;

        public AstGenerator(XCComponent component)
        {
            _component = component;
        }

        public void Build(AstBuilder builder)
        {
            _builder = builder;

            foreach (var type in _component.Types)
            {
                //На кадый класс - отдельный модуль
                var unit = _builder.WithUnit();
                var cl = unit.WithClass(type.Name);
            }
        }
    }
}