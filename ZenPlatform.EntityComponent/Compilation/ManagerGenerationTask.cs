using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.EntityComponent.Compilation
{
    public class ManagerGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        private QueryMachine _qm;
        public IPType ManagerType { get; }

        public ManagerGenerationTask(
            IPType managerType,
            CompilationMode compilationMode, IComponent component, string name,
            TypeBody tb)
            : base(compilationMode, component, true, name, tb)
        {
            ManagerType = managerType;
            _qm = new QueryMachine();
        }

        public RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm)
        {
            return asm.DefineInstanceType(GetNamespace(), Name);
        }

        public void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            EmitStructure(builder, dbType);
        }

        public void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitBody(builder, dbType);
        }

        public void EmitStructure(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            var managerType = ManagerType;
            var pLinkType = managerType.GetLinkType();
            var pObjectType = managerType.GetObjectType();

            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            var dtoClassName = managerType.GetDtoType().Name;
            var objectClassName = managerType.GetObjectType().Name;
            var linkClassName = managerType.GetLinkType().Name;


            var @namespace = Component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            var dtoType = ts.FindType($"{@namespace}.{dtoClassName}") ?? throw new Exception("Type not found");
            var objectType = ts.FindType($"{@namespace}.{objectClassName}") ?? throw new Exception("Type not found");
            var linkType = ts.FindType($"{@namespace}.{linkClassName}") ?? throw new Exception("Type not found");

            builder.DefineMethod("Create", true, true, false)
                .WithReturnType(objectType);

            //Get method
            builder.DefineMethod("Get", true, true, false)
                .WithReturnType(linkType)
                .DefineParameter("id", sb.Guid, false, false);

            builder.DefineMethod("Save", true, true, false)
                .DefineParameter("dto", dtoType, false, false);
        }

        private void EmitBody(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            var pManagerType = ManagerType;
            var pObjectType = pManagerType.GetObjectType();
            var xcLinkType = pManagerType.GetLinkType();

            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            var dtoClassName = pManagerType.GetDtoType().Name;
            var objectClassName = pManagerType.GetObjectType().Name;

            var linkClassName = xcLinkType.Name;


            var @namespace = Component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            var dtoType = ts.FindType($"{@namespace}.{dtoClassName}");
            var objectType = ts.FindType($"{@namespace}.{objectClassName}");
            var linkType = ts.FindType($"{@namespace}.{linkClassName}");

            var nGuid = sb.Guid.FindMethod(nameof(Guid.NewGuid));

            var create = (RoslynMethodBuilder) builder.FindMethod("Create");

            var cg = create.Body;

            var dto = cg.DefineLocal(dtoType);

            cg
                .NewObj(dtoType.FindConstructor())
                .StLoc(dto)
                .Statement()
                .LdLoc(dto)
                .Call(nGuid)
                .StProp(dtoType.FindProperty("Id"))
                .Statement();

            //init default values

            var byteArr = sb.Byte.MakeArrayType();

            foreach (var property in dtoType.Properties)
            {
                if (property.FindCustomAttribute<NeedInitAttribute>() != null)
                {
                    if (property.PropertyType == sb.DateTime)
                        cg.LdLoc(dto)
                            .LdDefaultDateTime()
                            .StProp(property).Statement();
                    else if (property.PropertyType.Equals(byteArr))
                    {
                        cg.LdLoc(dto)
                            .LdLit(0)
                            .NewArr(sb.Byte)
                            .StProp(property).Statement();
                    }
                    else if (property.PropertyType.Equals(sb.String))
                    {
                        cg.LdLoc(dto)
                            .LdLit("")
                            .StProp(property).Statement();
                    }
                }
            }

            cg.LdLoc(dto)
                .NewObj(objectType.FindConstructor(dtoType))
                .Ret().Statement()
                ;

            //Get method
            var get = (RoslynMethodBuilder) builder.FindMethod("Get", sb.Guid);

            var gg = get.Body;
            var dxcType = ts.Resolve<DbCommand>();
            var readerType = ts.Resolve<DbDataReader>();

            var pcolType = ts.Resolve<DbParameterCollection>();

            var parameterType = ts.Resolve<DbParameter>();
            var dxcLoc = gg.DefineLocal(dxcType);
            var readerLoc = gg.DefineLocal(readerType);
            var p_loc = gg.DefineLocal(parameterType);

            dto = gg.DefineLocal(dtoType);

            var q = GetSelectQuery(pObjectType);

            var compiler = SqlCompillerBase.FormEnum(dbType);


            // DbCommand d;
            //
            // d.ExecuteReader()

            gg
                .NewDbCmdFromContext()
                .StLoc(dxcLoc)
                .Statement()
                .LdLoc(dxcLoc)
                .LdLit(compiler.Compile(q))
                .StProp(sb.DbCommand.FindProperty(nameof(DbCommand.CommandText)))
                .Statement()

                //load parameter
                .LdLoc(dxcLoc)
                .Call(sb.DbCommand.FindMethod(nameof(DbCommand.CreateParameter)))
                .StLoc(p_loc)
                .Statement()
                .LdLoc(p_loc)
                .LdLit("P_0")
                .StProp(parameterType.FindProperty(nameof(DbParameter.ParameterName)))
                .Statement()
                .LdLoc(p_loc)
                .LdArg(get.Parameters[0].ArgIndex)
                //.Box(get.Parameters[0].Type)
                .StProp(parameterType.FindProperty(nameof(DbParameter.Value)))
                .Statement()
                .LdLoc(dxcLoc)
                .LdProp(dxcType.FindProperty(nameof(DbCommand.Parameters)))
                //collection on stack
                .LdLoc(p_loc)
                .Call(pcolType.FindMethod("Add", sb.Object))
                .Statement()

                //ExecuteReader        
                .LdLoc(dxcLoc)
                .Call(sb.DbCommand.FindMethod(nameof(DbCommand.ExecuteReader)))
                .StLoc(readerLoc)
                .Statement()
                .LdLoc(readerLoc)
                .Call(readerType.FindMethod(nameof(DbDataReader.Read)))
                .Statement()

                //Create dto and map it
                .NewObj(dtoType.FindConstructor())
                .StLoc(dto)
                .Statement()
                .LdLoc(dto)
                .LdLoc(readerLoc)
                .Call(dtoType.FindMethod("Map", readerType))
                .Statement()

                //release reader
                .LdLoc(readerLoc)
                .Call(readerType.FindMethod(nameof(DbDataReader.Dispose)))
                .Statement()

                //release command
                .LdLoc(dxcLoc)
                .Call(sb.DbCommand.FindMethod(nameof(DbCommand.Dispose)))
                .Statement()

                //Create link
                .LdLoc(dto)
                .NewObj(linkType.FindConstructor(dtoType))
                .Ret().Statement();

            EmitSavingSupport(builder, dbType);
        }

        private void EmitSavingSupport(RoslynTypeBuilder tb, SqlDatabaseType dbType)
        {
            var set = ManagerType;

            var pObjectType = set.GetObjectType();

            var ts = tb.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            var compiler = SqlCompillerBase.FormEnum(dbType);

            var dtoType = ts.FindType($"{set.GetNamespace()}.{set.GetDtoType().Name}");

            var saveMethod = (RoslynMethodBuilder) tb.FindMethod("Save", dtoType);

            var rg = saveMethod.Body;


            var cmdType = ts.Resolve<DbCommand>();
            var parameterType = ts.Resolve<DbParameter>();
            var pcolType = ts.Resolve<DbParameterCollection>();

            var dtoParam = saveMethod.Parameters[0];

            var indexp = 0;

            var p_loc = rg.DefineLocal(ts.Resolve<DbParameter>());

            var versionF = dtoType.Properties.First(x => x.Name == "Version");

            var cmdLoc = rg.DefineLocal(cmdType);

            if (versionF != null)
            {
                rg.NewDbCmdFromContext()
                    .StLoc(cmdLoc)
                    .Statement();

                rg.LdArg(dtoParam)
                    .LdProp(versionF)
                    .Null()
                    .Cneq()
                    .Block()
                    .LdLoc(cmdLoc)
                    .LdLit(compiler.Compile(GetUpdateQuery(pObjectType)))
                    .StProp(cmdType.FindProperty(nameof(DbCommand.CommandText)))
                    .Statement()
                    .EndBlock()
                    .Block()
                    .LdLoc(cmdLoc)
                    .LdLit(compiler.Compile(GetInsertQuery(pObjectType)))
                    .StProp(cmdType.FindProperty(nameof(DbCommand.CommandText)))
                    .Statement()
                    .EndBlock()
                    .TryIf()
                    .Statement();
            }

            foreach (var property in dtoType.Properties)
            {
                var m = property.FindCustomAttribute<MapToAttribute>();
                if (m is null) continue;

                rg.LdLoc(cmdLoc)
                    .Call(cmdType.FindMethod(nameof(DbCommand.CreateParameter)))
                    .StLoc(p_loc)
                    .Statement()
                    //add param to collection
                    .LdLoc(cmdLoc)
                    .LdProp(cmdType.FindProperty(nameof(DbCommand.Parameters)))
                    //collection on stack
                    .LdLoc(p_loc)
                    .Call(pcolType.FindMethod("Add", sb.Object))
                    .Statement()
                    //endadd
                    .LdLoc(p_loc)
                    .LdLit($"P_{indexp}")
                    .StProp(parameterType.FindProperty(nameof(DbParameter.ParameterName)))
                    .Statement()
                    .LdLoc(p_loc)
                    .LdArg_0()
                    .LdProp(property)
                    .StProp(parameterType.FindProperty(nameof(DbParameter.Value)))
                    .Statement();

                indexp++;
            }

            rg.LdLoc(cmdLoc)
                .Call(cmdType.FindMethod(nameof(DbCommand.ExecuteNonQuery)))
                .Statement();
        }

        private SSyntaxNode GetInsertQuery(IPType se)
        {
            QueryMachine qm = new QueryMachine();
            qm.bg_query()
                .m_values();

            var pIndex = 0;

            var columns = se.Properties.Where(x => !x.IsSelfLink)
                .SelectMany(x => x.GetDbSchema());

            foreach (var column in columns)
            {
                qm.ld_param($"P_{pIndex}");
                pIndex++;
            }

            qm.m_insert()
                .ld_table(se.GetSettings().DatabaseName);

            foreach (var col in columns)
            {
                qm.ld_column(col.FullName);
            }

            qm.st_query();

            return (SSyntaxNode) qm.pop();
        }

        private SSyntaxNode GetUpdateQuery(IPType se)
        {
            QueryMachine qm = new QueryMachine();

            var pIndex = 0;

            var columns = se.Properties.Where(x => !x.IsUnique && !x.IsReadOnly).SelectMany(x => x.GetDbSchema());

            qm.bg_query()
                .m_where()
                .ld_column(se.FindPropertyByName("Id").GetSettings().DatabaseName, "T0")
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
                .ld_table(se.GetSettings().DatabaseName)
                .@as("T0")
                .st_query();

            return (SSyntaxNode) qm.pop();
        }

        private SSyntaxNode GetSelectQuery(IPType se)
        {
            QueryMachine qm = new QueryMachine();

            var pIndex = 0;

            var columns = se.Properties.Where(x => !x.IsUnique).SelectMany(x => x.GetDbSchema());

            qm.bg_query()
                .m_from()
                .ld_table(se.GetSettings().DatabaseName)
                .@as("T0")
                .m_where()
                .ld_column(se.FindPropertyByName("Id").GetSettings().DatabaseName, "T0")
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