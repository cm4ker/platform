using System.Collections.Generic;
using ServiceStack;
using ZenPlatform.Core.Language.QueryLanguage.Model;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Model;
using Q = ZenPlatform.QueryBuilder.Builders.Query;

namespace ZenPlatform.Core.Language.QueryLanguage
{
    public class LogicToReal
    {
        public LogicToReal()
        {
        }

        public QuerySyntaxNode Build(QQuery query)
        {
            var builder = Q.New().Select();
            GenerateQuery(builder, query);
            //TODO: ISourceBuilder -> QuerySyntaxNode
            return null;
        }

        private void GenerateQuery(SelectBuilder b, QQuery q)
        {
        }

        private void GenerateSource(SelectBuilder sb, QObjectTable ot)
        {
            //TODO: Передаем управление компаненту
        }

        private void GenerateFrom(SelectBuilder b, QFrom from)
        {
            if (from.Source is QNestedQuery nq)
            {
                b.From(x => GenerateQuery(x, nq.Nested));
            }
            else if (from.Source is QObjectTable ot)
            {
                GenerateSource(b, ot);
            }
        }
    }
}