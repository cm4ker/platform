using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;

namespace ZenPlatform.XmlSerializer
{
    /// <summary>
    /// Конфигурация
    /// </summary>
    public class SerializerConfiguration
    {
        public static SerializerConfiguration Create() => new SerializerConfiguration();

        private SerializerConfiguration()
        {
            IgnoreTypes = new List<Type>();
            IgnoreProperties = new List<PropertyInfo>();
            Emitters = new Dictionary<Type, Action<object, XmlWriter>>();
        }

        /// <summary>
        /// Построить сериализатор на основании конфигурации
        /// </summary>
        /// <returns></returns>
        public IXmlSerializer Build()
        {
            return null;
        }

        /// <summary>
        /// Игнорируемые типы
        /// </summary>
        internal List<Type> IgnoreTypes { get; set; }

        internal List<PropertyInfo> IgnoreProperties { get; set; }

        internal Dictionary<Type, Action<object, XmlWriter>> Emitters;

        public SerializerConfiguration Ignore<T>()
        {
            return Ignore(typeof(T));
        }

        public SerializerConfiguration Ignore(Type type)
        {
            IgnoreTypes.Add(type);
            return this;
        }

        public SerializerConfiguration Ignore<T>(Expression<Func<T, object>> selector)
        {
            if (selector.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException("Selector must be lambda expression", "selector");
            }

            var lambda = (LambdaExpression) selector;

            var memberExpression = ExtractMemberExpression(lambda.Body);

            if (memberExpression == null)
            {
                throw new ArgumentException("Selector must be member access expression", "selector");
            }

            if (memberExpression.Member.DeclaringType == null)
            {
                throw new InvalidOperationException("Property does not have declaring type");
            }

            return Ignore(memberExpression.Member.DeclaringType.GetProperty(memberExpression.Member.Name));
        }

        /// <summary>
        /// Переопределить
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public SerializerConfiguration Emit(Type type, Action<object, XmlWriter> handler)
        {
            Emitters.Add(type, handler);
            return this;
        }

        public SerializerConfiguration Emit<T>(Action<object, XmlWriter> handler)
        {
            return Emit(typeof(T), handler);
        }

        /// <summary>
        /// Проигнорировать какое-нибудь свойство
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public SerializerConfiguration Ignore(PropertyInfo propertyInfo)
        {
            IgnoreProperties.Add(propertyInfo);
            return this;
        }

        #region Private

        private MemberExpression ExtractMemberExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return ((MemberExpression) expression);
            }

            if (expression.NodeType == ExpressionType.Convert)
            {
                var operand = ((UnaryExpression) expression).Operand;
                return ExtractMemberExpression(operand);
            }

            return null;
        }

        #endregion
    }

    public class Serializer : IXmlSerializer
    {
        private class XmlObjectWriter
        {
        }

        public void Serialize(object instance, TextWriter textWriter, SerializerConfiguration configuration = null)
        {
            using (var xw = XmlWriter.Create(textWriter))
                //TODO: Добавить параметры для XmlWriter
                Serialize(instance, xw, configuration);
        }

        public void Serialize(object instance, XmlWriter xmlWriter, SerializerConfiguration configuration = null)
        {
            var writer = new ObjectXmlWriter(instance, xmlWriter, configuration);
            writer.Handle();
        }

        public object Deserialize(string content, SerializerConfiguration configuration = null)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(TextReader reader, SerializerConfiguration configuration = null)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(XmlReader reader, SerializerConfiguration configuration = null)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(string content, SerializerConfiguration configuration = null)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(TextReader reader, SerializerConfiguration configuration = null)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(XmlReader reader, SerializerConfiguration configuration = null)
        {
            throw new NotImplementedException();
        }
    }


    public interface IXmlSerializer
    {
        void Serialize(object instance, TextWriter textWriter, SerializerConfiguration configuration = null);
        void Serialize(object instance, XmlWriter textWriter, SerializerConfiguration configuration = null);

        object Deserialize(string content, SerializerConfiguration configuration = null);
        object Deserialize(TextReader reader, SerializerConfiguration configuration = null);
        object Deserialize(XmlReader reader, SerializerConfiguration configuration = null);

        T Deserialize<T>(string content, SerializerConfiguration configuration = null);
        T Deserialize<T>(TextReader reader, SerializerConfiguration configuration = null);
        T Deserialize<T>(XmlReader reader, SerializerConfiguration configuration = null);
    }
}