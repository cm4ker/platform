using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            Attributes = new List<MemberInfo>();
            CustomRoots = new Dictionary<string, Type>();
            TypesWithoutRoot = new List<Type>();
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

        internal List<MemberInfo> Attributes;

        internal Dictionary<string, Type> CustomRoots;

        internal List<Type> TypesWithoutRoot;

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

        public SerializerConfiguration Attribute(Type type, MemberInfo memberInfo)
        {
            return this;
        }

        public SerializerConfiguration CustomRoot(Type type, string rootName)
        {
            CustomRoots.Add(rootName, type);
            return this;
        }

        public SerializerConfiguration CustomRoot<T>(string rootName)
        {
            return CustomRoot(typeof(T), rootName);
        }

        public SerializerConfiguration IgnoreRootInProperties(Type type)
        {
            TypesWithoutRoot.Add(type);
            return this;
        }

        public SerializerConfiguration IgnoreRootInProperties<T>()
        {
            return IgnoreRootInProperties(typeof(T));
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
}