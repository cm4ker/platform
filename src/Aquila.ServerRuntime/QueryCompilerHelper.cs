using System;
using System.IO;
using Antlr4.Runtime;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.Core.Querying;
using Aquila.Core.Querying.Model;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Symbols;
using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;

namespace Aquila.ServerRuntime
{
    public class QueryCompilerHelper
    {
        public static Class Query;

        public static Class DataReader;

        public static void Init(Root root, RoslynTypeSystem ts)
        {
            var pqType = ts.Resolve<PlatformQuery>();
            var drType = ts.Resolve<PlatformReader>();

            Query = new Class(null, TypeBody.Empty, "Query");
            DataReader = new Class(null, TypeBody.Empty, "DataReader");

            Query.SymbolScope = SymbolScopeBySecurity.Shared;
            Query.TypeBody.SymbolTable = new SymbolTable(root.SymbolTable);

            DataReader.SymbolScope = SymbolScopeBySecurity.Shared;
            DataReader.TypeBody.SymbolTable = new SymbolTable(root.SymbolTable);

            var prop = new Property(null, nameof(PlatformQuery.Text),
                new PrimitiveTypeSyntax(null, TypeNodeKind.String));

            root.SymbolTable.AddType(Query, pqType);
            root.SymbolTable.AddType(DataReader, drType);

            var propSym = Query.TypeBody.SymbolTable.AddProperty(prop);
            propSym.Connect(pqType.FindProperty(nameof(PlatformQuery.Text)));

            Query.TypeBody.SymbolTable.AddMethod(
                new Function(null, Block.Empty, ParameterList.Empty, GenericParameterList.Empty, AttributeList.Empty,
                    "ExecuteReader", new SingleTypeSyntax(null, "DataReader", TypeNodeKind.Type)),
                pqType.FindMethod(nameof(PlatformQuery.ExecuteReader)));

            DataReader.TypeBody.SymbolTable.AddMethod(
                new Function(null, Block.Empty, ParameterList.Empty, GenericParameterList.Empty, AttributeList.Empty,
                    "Read", new PrimitiveTypeSyntax(null, TypeNodeKind.Boolean)),
                drType.FindMethod(nameof(PlatformReader.Read)));
        }

        public static (string sql, QQueryList logicalTree) Compile(ITypeManager tm, string sql)
        {
            //need compile sql expression!
            var _m = new QLang(tm);

            AntlrInputStream inputStream = new AntlrInputStream(sql);
            ZSqlGrammarLexer speakLexer = new ZSqlGrammarLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            ZSqlGrammarParser parser = new ZSqlGrammarParser(commonTokenStream);
            ZSqlGrammarVisitor visitor = new ZSqlGrammarVisitor(_m);

            visitor.Visit(parser.parse());

            string sqlString = "";

            var logicalTree = _m.top() as QQueryList ??
                              throw new Exception("Query stack machine after parsing MUST return the QueryList");
            ;

            //Create aliases for tree
            var pwalker = new PhysicalNameWalker();
            pwalker.Visit(logicalTree);

            //Create query
            var realWalker = new RealWalker(_m.TypeManager);
            realWalker.Visit(logicalTree);

            var syntax = (realWalker.QueryMachine.pop() as SSyntaxNode);
            return (new SQLVisitorBase().Visit(syntax), logicalTree);
        }
    }
}