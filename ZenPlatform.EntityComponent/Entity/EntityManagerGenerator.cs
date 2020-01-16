using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;
using McMaster.Extensions.CommandLineUtils;
using NLog.LayoutRenderers;
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
using IField = ZenPlatform.Compiler.Contracts.IField;

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

        public void Stage1(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (astTree is ComponentModule cm)
            {
                if (cm.Bag != null && ((ObjectType) cm.Bag) == ObjectType.Manager)
                {
                    if (cm.CompilationMode.HasFlag(CompilationMode.Server) && mode.HasFlag(CompilationMode.Server))
                    {
                        EmitStructure(cm, builder, dbType);
                    }
                    else if (cm.CompilationMode.HasFlag(CompilationMode.Client))
                    {
                    }
                }
            }
        }

        public void Stage2(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
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

        public void EmitStructure(ComponentModule cm, ITypeBuilder builder, SqlDatabaseType dbType)
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

            builder.DefineMethod("Create", true, true, false)
                .WithReturnType(objectType);

            //Get method
            builder.DefineMethod("Get", true, true, false)
                .WithReturnType(linkType)
                .DefineParameter("id", sb.Guid, false, false);

            builder.DefineMethod("Save", true, true, false)
                .DefineParameter("dto", dtoType, false, false);
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

            var create = (IMethodBuilder) builder.FindMethod("Create");

            var cg = create.Generator;

            var dto = cg.DefineLocal(dtoType);

            cg
                .NewObj(dtoType.FindConstructor())
                .StLoc(dto)
                .LdLoc(dto)
                .EmitCall(nGuid)
                .EmitCall(dtoType.FindProperty("Id").Setter);

            //init default values

            var byteArr = sb.Byte.MakeArrayType();
            
            foreach (var property in dtoType.Properties)
            {
                if (property.FindCustomAttribute<NeedInitAttribute>() != null)
                {
                    if (property.PropertyType == sb.DateTime)
                        cg.LdLoc(dto)
                            .LdDefaultDateTime()
                            .EmitCall(property.Setter);
                    else if (property.PropertyType.Equals(byteArr))
                    {
                        cg.LdLoc(dto)
                            .LdcI4(0)
                            .NewArr(sb.Byte)
                            .EmitCall(property.Setter);
                    }
                }
            }

            cg.LdLoc(dto)
                .NewObj(objectType.FindConstructor(dtoType))
                .Ret()
                ;

            //Get method
            var get = (IMethodBuilder) builder.FindMethod("Get", sb.Guid);

            var gg = get.Generator;
            var dxcType = ts.FindType<DbCommand>();
            var readerType = ts.FindType<DbDataReader>();

            var pcolType = ts.FindType<DbParameterCollection>();

            var parameterType = ts.FindType<DbParameter>();
            var dxcLoc = gg.DefineLocal(dxcType);
            var readerLoc = gg.DefineLocal(readerType);
            var p_loc = gg.DefineLocal(parameterType);

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

                //load parameter
                .LdLoc(dxcLoc)
                .EmitCall(sb.DbCommand.FindMethod(nameof(DbCommand.CreateParameter)))
                .StLoc(p_loc)
                .LdLoc(p_loc)
                .LdStr("P_0")
                .EmitCall(parameterType.FindProperty(nameof(DbParameter.ParameterName)).Setter)
                .LdLoc(p_loc)
                .LdArg(get.Parameters[0].ArgIndex)
                .Box(get.Parameters[0].Type)
                .EmitCall(parameterType.FindProperty(nameof(DbParameter.Value)).Setter)
                .LdLoc(dxcLoc)
                .EmitCall(dxcType.FindProperty(nameof(DbCommand.Parameters)).Getter)
                //collection on stack
                .LdLoc(p_loc)
                .EmitCall(pcolType.FindMethod("Add", sb.Object), true)

                //ExecuteReader        
                .LdLoc(dxcLoc)
                .EmitCall(sb.DbCommand.FindMethod(nameof(DbCommand.ExecuteReader)))
                .StLoc(readerLoc)
                .LdLoc(readerLoc)
                .EmitCall(readerType.FindMethod(nameof(DbDataReader.Read)), true)

                //Create dto and map it
                .NewObj(dtoType.FindConstructor())
                .StLoc(dto)
                .LdLoc(dto)
                .LdLoc(readerLoc)
                .EmitCall(dtoType.FindMethod("Map", readerType))

                //release reader
                .LdLoc(readerLoc)
                .EmitCall(readerType.FindMethod(nameof(DbDataReader.Dispose)), true)

                //release command
                .LdLoc(dxcLoc)
                .EmitCall(sb.DbCommand.FindMethod(nameof(DbCommand.Dispose)), true)

                //Create link
                .LdLoc(dto)
                .NewObj(linkType.FindConstructor(dtoType))
                .Ret();

            EmitSavingSupport(cm, builder, dbType);
        }

        private void EmitSavingSupport(ComponentModule cm, ITypeBuilder tb, SqlDatabaseType dbType)
        {
            var set = cm.Type as XCSingleEntity ??
                      throw new Exception($"This component can't serve this type {cm.Type}");

            var ts = tb.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            var compiler = SqlCompillerBase.FormEnum(dbType);

            var dtoType = ts.FindType($"{set.GetNamespace()}.{set.GetDtoName()}");

            var saveMethod = (IMethodBuilder) tb.FindMethod("Save", dtoType);

            var rg = saveMethod.Generator;


            var cmdType = ts.FindType<DbCommand>();
            var parameterType = ts.FindType<DbParameter>();
            var pcolType = ts.FindType<DbParameterCollection>();

            var dtoParam = saveMethod.Parameters[0];

            var indexp = 0;

            var p_loc = rg.DefineLocal(ts.FindType<DbParameter>());

            var versionF = dtoType.Properties.First(x => x.Name == "Version");

            var cmdLoc = rg.DefineLocal(cmdType);

            if (versionF != null)
            {
                rg.NewDbCmdFromContext()
                    .StLoc(cmdLoc);

                var narg = rg.DefineLabel();
                var end = rg.DefineLabel();
                rg.LdArg(dtoParam)
                    .EmitCall(versionF.Getter)
                    .LdNull()
                    .Ceq()
                    .BrTrue(narg);

                rg
                    .LdLoc(cmdLoc)
                    .LdStr(compiler.Compile(GetUpdateQuery(set)))
                    .EmitCall(cmdType.FindProperty(nameof(DbCommand.CommandText)).Setter)
                    .Br(end);

                rg
                    .MarkLabel(narg)
                    .LdLoc(cmdLoc)
                    .LdStr(compiler.Compile(GetInsertQuery(set)))
                    .EmitCall(cmdType.FindProperty(nameof(DbCommand.CommandText)).Setter)
                    .MarkLabel(end);
            }

            foreach (var property in dtoType.Properties)
            {
                var m = property.FindCustomAttribute<MapToAttribute>();
                if (m is null) continue;

                rg.LdLoc(cmdLoc)
                    .EmitCall(cmdType.FindMethod(nameof(DbCommand.CreateParameter)))
                    .StLoc(p_loc)
                    //add param to collection
                    .LdLoc(cmdLoc)
                    .EmitCall(cmdType.FindProperty(nameof(DbCommand.Parameters)).Getter)
                    //collection on stack
                    .LdLoc(p_loc)
                    .EmitCall(pcolType.FindMethod("Add", sb.Object), true)
                    //endadd
                    .LdLoc(p_loc)
                    .LdStr($"P_{indexp}")
                    .EmitCall(parameterType.FindProperty(nameof(DbParameter.ParameterName)).Setter)
                    .LdLoc(p_loc)
                    .LdArg_0()
                    .EmitCall(property.Getter)
                    .Box(property.PropertyType)
                    .EmitCall(parameterType.FindProperty(nameof(DbParameter.Value)).Setter);

                indexp++;
            }

            rg.LdLoc(cmdLoc)
                .EmitCall(cmdType.FindMethod(nameof(DbCommand.ExecuteNonQuery)), true)
                .Ret();
        }

        private SSyntaxNode GetInsertQuery(XCSingleEntity se)
        {
            QueryMachine qm = new QueryMachine();
            qm.bg_query()
                .m_values();

            var pIndex = 0;

            var columns = se.Properties.Where(x => !x.IsSelfLink)
                .SelectMany(x => x.GetPropertySchemas());

            foreach (var column in columns)
            {
                qm.ld_param($"P_{pIndex}");
                pIndex++;
            }

            qm.m_insert()
                .ld_table(se.RelTableName);

            foreach (var col in columns)
            {
                qm.ld_column(col.FullName);
            }

            qm.st_query();

            return (SSyntaxNode) qm.pop();
        }

        private SSyntaxNode GetUpdateQuery(XCSingleEntity se)
        {
            QueryMachine qm = new QueryMachine();

            var pIndex = 0;

            var columns = se.Properties.Where(x => !x.Unique && !x.IsReadOnly).SelectMany(x => x.GetPropertySchemas());

            qm.bg_query()
                .m_where()
                .ld_column(se.GetPropertyByName("Id").DatabaseColumnName, "T0")
                .ld_param($"P_{pIndex++}")
                .eq();

            qm.m_set();
            Debug.Assert(columns.Any());

            foreach (var column in columns)
            {
                qm.ld_column(column.FullName, "T0")
                    .ld_param($"P_{pIndex++}")
                    .assign();
            }

            qm.m_update()
                .ld_table(se.RelTableName)
                .@as("T0")
                .st_query();

            return (SSyntaxNode) qm.pop();
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