using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.EntityComponent.Configuration;
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

            foreach (var xcObjectTypeBase in _component.Types)
            {
                var type = (XCSingleEntity) xcObjectTypeBase;
                BuildClass(builder, type);
            }
        }

        private void BuildClass(AstBuilder builder, XCSingleEntity type)
        {
            //На кадый класс - отдельный модуль
            var unit = _builder.WithUnit();
            var cl = unit.WithClass(type.Name);

            foreach (var property in type.Properties)
            {
                //cl.WithProperty();
            }
        }
    }
}