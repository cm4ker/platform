using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Helpers;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Contracts.Data;
using Aquila.Configuration.Contracts.TypeSystem;
using Aquila.EntityComponent.Entity;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.QueryBuilder;
using Aquila.QueryBuilder.Model;
using SystemTypeBindings = Aquila.Compiler.Roslyn.SystemTypeBindings;

namespace Aquila.EntityComponent.Compilation
{
    public class ManagerGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        private QueryMachine _qm;
        private RoslynTypeBuilder _builder;
        private RoslynTypeSystem _ts;
        private SystemTypeBindings _sb;
        private RoslynType _dtoType;
        private RoslynType _objectType;
        private RoslynType _linkType;

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
            _ts = asm.TypeSystem;
            _sb = _ts.GetSystemBindings();
            return _builder = asm.DefineInstanceType(GetNamespace(), Name);
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

            _dtoType = ts.FindType($"{@namespace}.{dtoClassName}") ?? throw new Exception("Type not found");
            _objectType = ts.FindType($"{@namespace}.{objectClassName}") ?? throw new Exception("Type not found");
            _linkType = ts.FindType($"{@namespace}.{linkClassName}") ?? throw new Exception("Type not found");

            builder.DefineMethod("Create", true, true, false)
                .WithReturnType(_objectType);

            //Get method
            builder.DefineMethod("Get", true, true, false)
                .WithReturnType(_linkType)
                .DefineParameter("id", sb.Guid, false, false);

            builder.DefineMethod("GetDto", true, true, false)
                .WithReturnType(_dtoType)
                .DefineParameter("id", sb.Guid, false, false);

            builder.DefineMethod("Save", true, true, false)
                .DefineParameter("dto", _dtoType, false, false);
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
                .LdLoc(dto)
                .Call(nGuid)
                .StProp(dtoType.FindProperty("Id"))
                ;

            //init default values

            var byteArr = sb.Byte.MakeArrayType();

            foreach (var property in dtoType.Properties)
            {
                if (property.FindCustomAttribute<NeedInitAttribute>() != null)
                {
                    if (property.PropertyType == sb.DateTime)
                        cg.LdLoc(dto)
                            .LdDefaultDateTime()
                            .StProp(property);
                    else if (property.PropertyType.Equals(byteArr))
                    {
                        cg.LdLoc(dto)
                            .LdLit(0)
                            .NewArr(sb.Byte)
                            .StProp(property);
                    }
                    else if (property.PropertyType.Equals(sb.String))
                    {
                        cg.LdLoc(dto)
                            .LdLit("")
                            .StProp(property);
                    }
                }
            }

            cg.LdLoc(dto)
                .NewObj(objectType.FindConstructor(dtoType))
                .Ret()
                ;

            EmitGet(builder, dbType, sb, ts, dtoType, pObjectType);
            EmitGetDto(dbType);
            EmitSave(builder, dbType);
        }

        private void EmitGet(RoslynTypeBuilder builder, SqlDatabaseType dbType, SystemTypeBindings sb,
            RoslynTypeSystem ts,
            RoslynType dtoType, IPType pObjectType)
        {
            RLocal dto;
            //Get method
            var get = (RoslynMethodBuilder) builder.FindMethod("Get", sb.Guid);

            var gg = get.Body;
            gg
                .LdArg(0)
                .NewObj(_linkType.FindConstructor(sb.Guid))
                .Ret();
        }

        private void EmitGetDto(SqlDatabaseType dbType)
        {
            //Get method
            var get = (RoslynMethodBuilder) _builder.FindMethod("GetDto", _sb.Guid);

            var gg = get.Body;
            var dxcType = _ts.Resolve<DbCommand>();
            var readerType = _ts.Resolve<DbDataReader>();

            var pcolType = _ts.Resolve<DbParameterCollection>();

            var parameterType = _ts.Resolve<DbParameter>();
            var dxcLoc = gg.DefineLocal(dxcType);
            var readerLoc = gg.DefineLocal(readerType);
            var p_loc = gg.DefineLocal(parameterType);

            var dto = gg.DefineLocal(_dtoType);

            var q = GetSelectQuery(ManagerType.GetObjectType());

            var compiler = SqlCompillerBase.FormEnum(dbType);


            gg
                .NewDbCmdFromContext()
                .StLoc(dxcLoc)
                .LdLoc(dxcLoc)
                .LdLit(compiler.Compile(q))
                .StProp(_sb.DbCommand.FindProperty(nameof(DbCommand.CommandText)))

                //load parameter
                .LdLoc(dxcLoc)
                .Call(_sb.DbCommand.FindMethod(nameof(DbCommand.CreateParameter)))
                .StLoc(p_loc)
                .LdLoc(p_loc)
                .LdLit("P_0")
                .StProp(parameterType.FindProperty(nameof(DbParameter.ParameterName)))
                .LdLoc(p_loc)
                .LdArg(get.Parameters[0].ArgIndex)
                //.Box(get.Parameters[0].Type)
                .StProp(parameterType.FindProperty(nameof(DbParameter.Value)))
                .LdLoc(dxcLoc)
                .LdProp(dxcType.FindProperty(nameof(DbCommand.Parameters)))
                //collection on stack
                .LdLoc(p_loc)
                .Call(pcolType.FindMethod("Add", _sb.Object))

                //ExecuteReader        
                .LdLoc(dxcLoc)
                .Call(_sb.DbCommand.FindMethod(nameof(DbCommand.ExecuteReader)))
                .StLoc(readerLoc)
                .LdLoc(readerLoc)
                .Call(readerType.FindMethod(nameof(DbDataReader.Read)))

                //Create dto and map it
                .NewObj(_dtoType.FindConstructor())
                .StLoc(dto)
                .LdLoc(dto)
                .LdLoc(readerLoc)
                .Call(_dtoType.FindMethod("Map", readerType))

                //release reader
                .LdLoc(readerLoc)
                .Call(readerType.FindMethod(nameof(DbDataReader.Dispose)))

                //release command
                .LdLoc(dxcLoc)
                .Call(_sb.DbCommand.FindMethod(nameof(DbCommand.Dispose)))
                .LdLoc(dto)
                .Ret();
        }

        private void EmitSave(RoslynTypeBuilder tb, SqlDatabaseType dbType)
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
                    ;

                rg.LdArg(dtoParam)
                    .LdProp(versionF)
                    .Null()
                    .Cneq()
                    .Block()
                    .LdLoc(cmdLoc)
                    .LdLit(compiler.Compile(GetUpdateQuery(pObjectType)))
                    .StProp(cmdType.FindProperty(nameof(DbCommand.CommandText)))
                    .EndBlock()
                    .Block()
                    .LdLoc(cmdLoc)
                    .LdLit(compiler.Compile(GetInsertQuery(pObjectType)))
                    .StProp(cmdType.FindProperty(nameof(DbCommand.CommandText)))
                    .EndBlock()
                    .If()
                    ;
            }

            foreach (var property in dtoType.Properties)
            {
                var m = property.FindCustomAttribute<MapToAttribute>();
                if (m is null) continue;

                rg.LdLoc(cmdLoc)
                    .Call(cmdType.FindMethod(nameof(DbCommand.CreateParameter)))
                    .StLoc(p_loc)

                    //add param to collection
                    .LdLoc(cmdLoc)
                    .LdProp(cmdType.FindProperty(nameof(DbCommand.Parameters)))
                    //collection on stack
                    .LdLoc(p_loc)
                    .Call(pcolType.FindMethod("Add", sb.Object))

                    //endadd
                    .LdLoc(p_loc)
                    .LdLit($"P_{indexp}")
                    .StProp(parameterType.FindProperty(nameof(DbParameter.ParameterName)))
                    .LdLoc(p_loc)
                    .LdArg_0()
                    .LdProp(property)
                    .StProp(parameterType.FindProperty(nameof(DbParameter.Value)))
                    ;

                indexp++;
            }

            rg.LdLoc(cmdLoc)
                .Call(cmdType.FindMethod(nameof(DbCommand.ExecuteNonQuery)))
                ;
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