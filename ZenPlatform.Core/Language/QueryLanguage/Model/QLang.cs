using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Atn;
using Newtonsoft.Json.Bson;
using ServiceStack;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage
{
    public class BeginQueryToken
    {
    }

    public abstract class StackObject
    {
    }


    public class QDataSource : StackObject
    {
        
    }

    public class Alias
    {
    }

    public class QLang
    {
        private readonly XCRoot _conf;
        private InstructionContext _instructionContext = InstructionContext.None;
        private QueryContext _queryContext;


        private enum QueryContext
        {
            None,
            Select,
            From,
            GroupBy,
            Having,
            Where
        }

        private enum InstructionContext
        {
            None,
            Component
        }


        private Stack<object> _logicStack;

        public QLang(XCRoot conf)
        {
            _conf = conf;
        }

        #region Context modifiers

        public void m_from()
        {
        }

        public void m_select()
        {
        }

        public void m_where()
        {
        }

        public void m_group_by()
        {
        }

        public void m_having()
        {
        }

        #endregion

        #region Load instructions

        public void ld_component(string componentName)
        {
        }

        public void ld_type(string typeName)
        {
        }


        /// <summary>
        /// Записать источник данных. Ссылка на него полностью получается из метаданных 
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="typeName"></param>
        /// <param name="alias"></param>
        public void ld_source(string componentName, string typeName, string p_alias = "")
        {
            ld_component(componentName);
            ld_type(typeName);

            if (!p_alias.IsNullOrEmpty())
            {
                alias(p_alias);
            }
        }

        public void ld_name(string name)
        {
        }

        public void ld_field()
        {
        }

        #endregion

        public void alias(string alias)
        {
        }

        public void beign_query()
        {
            if (_queryContext == QueryContext.None)
                _logicStack.Push(new BeginQueryToken());
        }

        private void pop()
        {
            _logicStack.Pop();
        }
    }

    /*
     
    begin_query 
    
    m_from
    
    ld_component "Document"
    ld_type "Invoice"
    alias "A"
    ld_component "Reference"
    ld_type "Contractors"
    alias "B"
    
    ld_name "A" <-- name context
    ld_field "Id"                                SELECT SUM(A.Count) AS SumOfCount
                                            =>   FROM Document.Invoice A
    ld_name "B" <-- name context                    JOIN Reference.Contractors B
    ld_field "Id"                                        ON A.Id = B.Id
    
    eq
    
    on 
    
    join    
    
    m_select
    
    ld_name "A"
    ld_field "Count" 
    sum
    alias "SumOfCount" <------ depend on context automatically wrap into SelectAliasedExpr Or SourceAliasedExpr 

    st_field <---save complex field on the stack
        
    st_query <--- save query on  the stack
    
    */
}