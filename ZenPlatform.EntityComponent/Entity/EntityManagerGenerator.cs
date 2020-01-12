using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Data;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Model;
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
        
        EntityLink Get(Guid g)
        {
             var cmd = "SELECT Id, .... FROM Obj_0001 WHERE Id = @id"
             var context = Context.Session.GetDbManager()       
        }  
     }
     
     
     */
    public class EntityManagerGenerator
    {
        private readonly IXCComponent _component;
        private readonly QueryMachine _qm;

        public EntityManagerGenerator(IXCComponent component)
        {
            _component = component;
            _qm = new QueryMachine();
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
            var type = (XCSingleEntity) cm.Type;
            var xcLinkType = _component.Types.FirstOrDefault(x => x is IXCLinkType lt && lt.ParentType == type);
            var set = cm.Type as XCSingleEntity ?? throw new Exception("This component can generate only SingleEntity");
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            var dtoClassName =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";
            var objectClassName = $"{type.Name}";

            var linkClassName = xcLinkType.Name;


            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            var dtoType = ts.FindType($"{@namespace}.{dtoClassName}");
            var objectType = ts.FindType($"{@namespace}.{objectClassName}");
            var linkType = ts.FindType($"{@namespace}.{linkClassName}");

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

            //Get method
            var get = builder.DefineMethod("Get", true, true, false)
                .WithReturnType(linkType);

            var guidParam = get.DefineParameter("id", sb.Guid, false, false);

            var gg = get.Generator;
            var dxcType = ts.FindType<DbCommand>();
            var readerType = ts.FindType<DbDataReader>();
            var dxcLoc = gg.DefineLocal(dxcType);
            var readerLoc = gg.DefineLocal(readerType);

            dto = gg.DefineLocal(dtoType);

            var q = GetSelectQuery(set);

            var compiler = SqlCompillerBase.FormEnum(dbType);


            // DbCommand d;
            //
            // d.ExecuteReader()

            gg
                .NewDbCmdFromContext()
                .StLoc(dxcLoc)
                .LdLoc(dxcLoc)
                .LdStr(compiler.Compile(q))
                .EmitCall(sb.DbCommand.FindProperty(nameof(DbCommand.CommandText)).Setter)
                //ExecuteReader        
                .LdLoc(dxcLoc)
                .EmitCall(sb.DbCommand.FindMethod(nameof(DbCommand.ExecuteReader)))
                .StLoc(readerLoc)
                //Create dto and map it
                .NewObj(dtoType.FindConstructor())
                .StLoc(dto)
                .LdLoc(dto)
                .LdLoc(readerLoc)
                .EmitCall(dtoType.FindMethod("Map", readerType))
                //Create link
                .LdLoc(dto)
                .NewObj(linkType.FindConstructor(dtoType))
                .Ret();
        }


        private SSyntaxNode GetSelectQuery(XCSingleEntity se)
        {
            QueryMachine qm = new QueryMachine();

            var pIndex = 0;

            var columns = se.Properties.Where(x => !x.Unique).SelectMany(x => x.GetPropertySchemas());

            qm.bg_query()
                .m_from()
                .ld_table(se.RelTableName)
                .@as("T0")
                .m_where()
                .ld_column(se.GetPropertyByName("Id").DatabaseColumnName, "T0")
                .ld_param($"P_{pIndex}")
                .eq();


            qm.m_select();

            Debug.Assert(columns.Any());

            foreach (var column in columns)
            {
                qm.ld_column(column.FullName, "T0");
            }


            qm.st_query();
            return (SSyntaxNode) qm.pop();
        }
    }
}