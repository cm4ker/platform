using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.DML.Select;

namespace ZenPlatform.Core.Language.QueryLanguage
{
    /// <summary>
    /// Обходит дерево запроса платформы и распарсивает все его части. Также отправляет запросы всем комопнентам на разворот
    /// </summary>
    public class ZSqlGrammarVisitor : ZSqlGrammarBaseVisitor<SqlNode>
    {
        private SqlNode _result;

        private SqlNode _context;

        private Dictionary<string, string> _aliases;


        public ZSqlGrammarVisitor()
        {
            _result = new SelectQueryNode();
            _aliases = new Dictionary<string, string>();
        }

        public override SqlNode VisitExpr(ZSqlGrammarParser.ExprContext context)
        {
            if (context.object_name() != null && context.component_name() != null)
            {
                if (_aliases.TryGetValue(context.component_name().GetText(), out var componentName))
                {
                    var objectName = context.object_name().GetText();

                    //TODO: Необходимо отдать компоненту данные либо получить определение от компонента
                }
                else
                {
                    // Нужно как-то поругаться, что не нашли компонент
                    //throw new Exception("if");
                }

            }
            else if (context.object_name() != null)
            {

            }

            return base.VisitExpr(context);
        }

        public override SqlNode VisitParse(ZSqlGrammarParser.ParseContext context)
        {
            base.VisitParse(context);
            return _result;
        }
    }
}
