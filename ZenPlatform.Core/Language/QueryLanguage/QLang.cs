using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Atn;
using Newtonsoft.Json.Bson;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage
{
    public class StartQueryToken
    {
    }


    public abstract class StackObject
    {
    }

    public class QLang
    {
        private readonly XCRoot _conf;
        private readonly StateContext _stateToken = StateContext.None;


        private enum StateContext
        {
            None,
            Component
        }


        private Stack<object> _logicStack;

        public QLang(XCRoot conf)
        {
            _conf = conf;
        }


        public void ld_component(string componentName)
        {
            Push();
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
        public void ld_source(string componentName, string typeName, string alias = "")
        {
            ld_component(componentName);
            ld_type(typeName);
        }

        private void Push(object obj)
        {
            if (_stateToken == StateToken.None)
                _logicStack.Push(new StartQueryToken());

            _logicStack.Push(obj);
        }
    }

    /*
     
    begin_query 
    
    ld_source "Document.Invoice" can be splited on(ld_component and ld_object)
    alias "A"
    ld_source "Reference.Contractors"
    alias "B"
    
    ld_name "A" <-- name context
    ld_field "Id"                                SELECT SUM(A.Count)
                                            =>   FROM Document.Invoice A
    ld_name "B" <-- name context                    JOIN Reference.Contractors B
    ld_field "Id"                                        ON A.Id = B.Id
    
    eq
    
    on 
    
    join    
    
    ld_name "A"
    ld_field "Count" 
    sum
    
    st_query <--- save query on  the stack
    
    */
}