using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.DataComponent;
using ZenPlatform.EntityComponent.Compilation;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.EntityComponent.Migrations;
using ZenPlatform.EntityComponent.QueryBuilders;
using ZenPlatform.EntityComponent.UIGenerations;

namespace ZenPlatform.EntityComponent
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