using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.EntityComponent.Entity
{
    
    /*
     
     Manager
     {
        Entity Create()
        {
            var dto  = new dto();
            dto.Id = Guid.NewId();
            return new Entity(dto);
        }
        
        EntityLink Get()
        {
            var dto = DbManager.GetDto();
            
            
            
            return new Link(dto);            
        }
     
     }
     
     
     */
    public class EntityManagerGenerator
    {
        private readonly IXCComponent _component;

        public EntityManagerGenerator(IXCComponent component)
        {
            _component = component;
        }

        public void GenerateAstTree(IXCObjectType type, Root root)
        {
            var className = type.Name + "Manager";

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            var cls = new ComponentModule(CompilationMode.Server, _component, type, null, className,
                new TypeBody(new List<Member>()));
            cls.Bag = ObjectType.Manager;

            cls.Namespace = @namespace;

            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});

            root.Add(cu);
        }

        public void EmitDetail(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (astTree is ComponentModule cm)
            {
                if (cm.Bag != null && ((ObjectType) cm.Bag) == ObjectType.Manager)
                {
                    if (cm.CompilationMode.HasFlag(CompilationMode.Server) && mode.HasFlag(CompilationMode.Server))
                    {
                        EmitBody(cm, builder, dbType);
                    }
                    else if (cm.CompilationMode.HasFlag(CompilationMode.Client))
                    {
                    }
                }
            }
        }

        private void EmitBody(ComponentModule cm, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var type = cm.Type;
            var set = cm.Type as XCSingleEntity ?? throw new Exception("This component can generate only SingleEntity");
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            var dtoClassName =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";
            var objectClassName = $"{type.Name}";

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            var dtoType = ts.FindType($"{@namespace}.{dtoClassName}");
            var objectType = ts.FindType($"{@namespace}.{objectClassName}");
            var nGuid = sb.Guid.FindMethod(nameof(Guid.NewGuid));

            var create = builder.DefineMethod("Create", true, true, false)
                .WithReturnType(objectType);

            var cg = create.Generator;

            var dto = cg.DefineLocal(dtoType);
            
            cg
                .NewObj(dtoType.FindConstructor())
                .StLoc(dto)
                
                .LdLoc(dto)
                .EmitCall(nGuid)
                .EmitCall(dtoType.FindProperty("Id").Setter)
                
                .LdLoc(dto)
                .NewObj(objectType.FindConstructor(dtoType))
                .Ret()
                ;
        }
    }
}