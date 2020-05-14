using Aquila.Configuration.Contracts.Data;
using Aquila.Configuration.Contracts.TypeSystem;
using Aquila.DataComponent;
using Aquila.EntityComponent.Compilation;
using Aquila.EntityComponent.Configuration;
using Aquila.EntityComponent.Entity;
using Aquila.EntityComponent.Migrations;
using Aquila.EntityComponent.QueryBuilders;
using Aquila.EntityComponent.UIGenerations;

namespace Aquila.EntityComponent
{
    public class EntityComponent : DataComponentBase
    {
        public EntityComponent(IComponent component) : base(component)
        {
        }

        public override void OnInitializing()
        {
            //Generator = new StagedGenerator(Component);
            //Manager = new SingleEntityManager();
            
            //TODO: Вынести интерфейс генерации UI в DataComponentBase. Если мы взаимодействуем с данными, то мы должны их как-то показывать
            InterfaceGenerator = new InterfaceGenerator();
            
            QueryInjector = new SingleEntityQueryInjector(Component);

            Generator = new EntityPlatformGenerator(Component); // new StagedGeneratorAst(Component);

            Migrator = new SingleEntityMigrator();

            Loader = new ComponentManager();
        
            RegisterSupportedTypes();
            RegisterCodeRules();

            base.OnInitializing();
        }

        public InterfaceGenerator InterfaceGenerator { get; private set; }

        private void RegisterSupportedTypes()
        {
            //Выпилено 
            // SupportedTypesManager.RegisterType(typeof(PDocumentObjectType));
        }

        private void RegisterCodeRules()
        {
            Component.RegisterCodeRule(new CodeGenRule(CodeGenRuleType.DtoPostfixRule, ""));
            Component.RegisterCodeRule(new CodeGenRule(CodeGenRuleType.DtoPreffixRule, "_"));
            Component.RegisterCodeRule(new CodeGenRule(CodeGenRuleType.EntityClassPostfixRule, ""));
            Component.RegisterCodeRule(new CodeGenRule(CodeGenRuleType.EntityClassPrefixRule, ""));
            Component.RegisterCodeRule(new CodeGenRule(CodeGenRuleType.NamespaceRule, "Entity"));
        }
    }
}