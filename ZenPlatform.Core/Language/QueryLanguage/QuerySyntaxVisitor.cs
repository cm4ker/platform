using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Enumeration;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MoreLinq.Extensions;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.ParenChildCollection;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Core.Language.QueryLanguage
{
    /// <summary>
    /// Обходит дерево запроса платформы и распарсивает все его части. Также отправляет запросы всем комопнентам на разворот
    /// </summary>
    public class ZSqlGrammarVisitor : ZSqlGrammarBaseVisitor<LTItem>
    {
        private SqlNode _result;
        private DataQueryConstructorContext _context;
        private XCRoot _conf;
        private Stack<LTItem> _dependencyStack;

        public ZSqlGrammarVisitor(XCRoot configuration, DataQueryConstructorContext context)
        {
            _result = new SelectQueryNode();
            _conf = configuration;
            _context = context;
        }
    }
}