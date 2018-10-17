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

        internal Dictionary<Type, Action<object, XmlWriter>> _handlers;

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
        public SerializerConfiguration OverrideWrite(Type type, Action<object, XmlWriter> handler)
        {
            return this;
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

    public class XmlSerializer : IXmlSerializer
    {
        /// <summary>
        /// Внутренний класс для обработки объекта в xml представление
        /// </summary>
        internal class ObjectXmlWriter
        {
            private readonly object _instance;
            private readonly XmlWriter _xmlWriter;
            private readonly Type _type;

            public ObjectXmlWriter(object instance, XmlWriter xmlWriter)
            {
                _instance = instance;
                _xmlWriter = xmlWriter;
                _type = instance.GetType();
            }

            public void Handle()
            {
                //Поведение по умолчанию: просто интерпретируем тип как имя элемента
                _xmlWriter.WriteStartElement(_type.Name);
                HandleFields();
                HandleProperties();
                _xmlWriter.WriteEndElement();
            }

            #region Field handling

            private void HandleFields()
            {
                foreach (var field in _type.GetFields(BindingFlags.Public))
                {
                    var value = field.GetValue(_instance);
                    HandleObjectField(field.Name, value);
                }
            }

            #endregion

            #region Property handling

            private void HandleProperties()
            {
                foreach (var prop in _type.GetProperties())
                {
                    var value = prop.GetValue(_instance);
                    HandleObjectField(prop.Name, value);
                }
            }

            #endregion

            private void HandleObjectField(string name, object value)
            {
                _xmlWriter.WriteStartElement(name);
                HandleValue(value);
                _xmlWriter.WriteEndElement();
            }

            private void HandleValue(object value)
            {
                if (value == null) return;
                Type type = value.GetType();

                switch (value)
                {
                    case bool _:
                    case byte _:
                    case byte[] _:
                    case DateTime _:
                    case DateTimeOffset _:
                    case decimal _:
                    case double _:
                    case short _:
                    case int _:
                    case long _:
                    case sbyte _:
                    case float _:
                    case string _:
                    case TimeSpan _:
                    case ushort _:
                    case uint _:
                    case ulong _:
                        _xmlWriter.WriteValue(value);
                        break;
                    default:
                        new ObjectXmlWriter(value, _xmlWriter).Handle();
                        break;
                }
            }
        }


        public void Serialize(object instance, TextWriter textWriter)
        {
            using (var xw = XmlWriter.Create(textWriter))
                //TODO: Добавить параметры для XmlWriter
                Serialize(instance, xw);
        }

        public void Serialize(object instance, XmlWriter xmlWriter)
        {
            var writer = new ObjectXmlWriter(instance, xmlWriter);
            writer.Handle();
        }
    }


    public interface IXmlSerializer
    {
        void Serialize(object instance, TextWriter textWriter);
        void Serialize(object instance, XmlWriter textWriter);
    }
}