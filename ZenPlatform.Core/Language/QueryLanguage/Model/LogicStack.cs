using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class LogicStack : Stack<object>
    {
        public XCComponent PopComponent()
        {
            return (XCComponent) this.Pop();
        }

        public IQDataSource PopDataSource()
        {
            return (IQDataSource) this.Pop();
        }

        public QField PopField()
        {
            return (QField) this.Pop();
        }

        public QExpression PopExpression()
        {
            return (QExpression) this.Pop();
        }

        public QFrom PopFrom()
        {
            return (QFrom) this.Pop();
        }

        public QOn PopOn()
        {
            return (QOn) this.Pop();
        }
        
        public QQuery PopQuery()
        {
            return (QQuery) this.Pop();
        }

        /// <summary>
        /// Получить непрерывную последовательность элементов одного типа
        /// </summary>
        /// <typeparam name="T">Тип элементов</typeparam>
        /// <returns>Коллекция элементов</returns>
        public List<T> PopItems<T>()
        {
            var result = new List<T>();
            while (true)
            {
                if (Peek() is T item)
                {
                    result.Add(item);
                    Pop();
                }
                else break;
            }

            return result;
        }


    }
}