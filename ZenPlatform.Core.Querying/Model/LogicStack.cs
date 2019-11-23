using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data;

namespace ZenPlatform.Core.Querying.Model
{
    public class LogicStack : Stack<object>
    {
        public IXCComponent PopComponent()
        {
            return (IXCComponent) this.Pop();
        }

        public QDataSource PopDataSource()
        {
            return (QDataSource) this.Pop();
        }

        public QField PopField()
        {
            return (QField) this.Pop();
        }

        public QExpression PopExpression()
        {
            return (QExpression) this.Pop();
        }

        public QOperationExpression PopOpExpression()
        {
            return (QOperationExpression) this.Pop();
        }

        public QFrom PopFrom()
        {
            return (QFrom) this.Pop();
        }

        public QQuery PopQuery()
        {
            return (QQuery) this.Pop();
        }

        /// <summary>
        /// Возвращает со стэка все поля, если попадается expression, он оборачивает его в поле и возврящает 
        /// </summary>
        /// <returns></returns>
        public List<QField> PopFields()
        {
            var result = new List<QField>();

            if (Count > 0)
                while (true && Count > 0)
                {
                    if (Peek() is QField item)
                    {
                        result.Add(new QSelectExpression(item));
                        Pop();
                    }
                    else if (Peek() is QExpression exp)
                    {
                        result.Add(new QSelectExpression(exp));
                        Pop();
                    }
                    else break;
                }

            return result;
        }

        /// <summary>
        /// Получить непрерывную последовательность элементов одного типа
        /// </summary>
        /// <typeparam name="T">Тип элементов</typeparam>
        /// <returns>Коллекция элементов</returns>
        public List<T> PopItems<T>()
        {
            var result = new List<T>();

            if (Count > 0)
                while (true && Count > 0)
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

        /// <summary>
        /// Пытается поднять сверху объект типа <see cref="T"/>, если сверху нет этого объекта, значит ничего не происходит
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T PopItem<T>()
        {
            if (Count > 0)
                if (Peek() is T item)
                {
                    Pop();
                    return item;
                }

            return default;
        }

        public IXCType PopType()
        {
            return (IXCType) this.Pop();
        }
    }
}