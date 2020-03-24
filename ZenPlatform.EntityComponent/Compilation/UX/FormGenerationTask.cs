using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Compilation.UX
{
    public class FormGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        private readonly MDInterface _md;

        public FormGenerationTask(
            IPType objectType, MDInterface md, CompilationMode compilationMode, IComponent component, bool isModule,
            string name,
            TypeBody tb) : base(compilationMode, component, isModule, name, tb)
        {
            _md = md;
            ObjectType = objectType;
        }

        public MDInterface MD { get; }

        public IPType ObjectType { get; }

        /*
         
         Server
            Form
                Model + UXObject
         
         .ctor
         {
            UXObj = new UXForm()
            
            //Some user initialized code
            
            Model = model
         }
         */

        public ITypeBuilder Stage0(IAssemblyBuilder asm)
        {
            return asm.DefineStaticType(GetNamespace(), ObjectType.Name + Name + "Form");
        }

        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var m = builder.DefineMethod("Show", true, true, false);


            var g = m.Generator;

            var xaml = g.DefineLocal(builder.Assembly.TypeSystem.GetSystemBindings().String);


            g.LdStr(_md.Markup)
                .StLoc(xaml)
                .Ret();
        }

        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}