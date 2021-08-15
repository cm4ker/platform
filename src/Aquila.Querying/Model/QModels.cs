using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.Metadata;

namespace Aquila.Core.Querying.Model
{
    public partial class QAliasedDataSource : QDataSource
    {
        private List<QField> _fields;

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= ParentSource.GetFields().Select(x => (QField)new QIntermediateSourceField(x, this))
                .ToList();
        }

        public override string ToString()
        {
            return ParentSource + " AS " + Alias;
        }
    }

    public partial class QAliasedSelectExpression : QSelectExpression
    {
        public override string GetName()
        {
            return Alias;
        }

        public override string ToString()
        {
            return $"{Expression} AS {Alias}";
        }
    }

    public partial class QCase : QExpression
    {
        public override IEnumerable<SMType> GetExpressionType()
        {
            var unionType = Whens.SelectMany(x => x.Then.GetExpressionType()).Concat(Else.GetExpressionType())
                .Distinct();

            return unionType;
        }
    }

    public partial class QCast
    {
        public override IEnumerable<SMType> GetExpressionType()
        {
            yield return Type;
        }

        public override string ToString()
        {
            return $"Cast: {string.Join(',', BaseExpression.GetExpressionType().Select(x => x.Name))} -> {Type.Name}";
        }
    }

    /// <summary>
    /// Константное значение
    /// </summary>
    public partial class QConst : QExpression
    {
        private readonly SMType _baseSMType;


        public QConst(SMType baseSMType, object value)
        {
            Value = value;
            _baseSMType = baseSMType;
        }

        public object Value { get; }

        public override IEnumerable<SMType> GetExpressionType()
        {
            yield return _baseSMType;
        }

        public override string ToString()
        {
            return $"Const = {Value}";
        }
    }

    public partial class QDataSource
    {
        public virtual IEnumerable<QField> GetFields()
        {
            return null;
        }

        public virtual IEnumerable<QTable> GetTables()
        {
            return null;
        }

        public QTable FindTable(string name)
        {
            return GetTables().FirstOrDefault(x => x.Name == name);
        }
    }

    /// <summary>
    /// Произвольное выражение
    /// </summary>
    public partial class QExpression
    {
        public virtual IEnumerable<SMType> GetExpressionType()
        {
            yield break;
        }

        public override string ToString()
        {
            return "Expression";
        }
    }

    /// <summary>
    /// Поле
    /// </summary>
    public abstract partial class QField : QExpression
    {
        public virtual string GetName()
        {
            return "Unknown";
        }
    }

    /// <summary>
    /// Представляет собой логическую структуру запроса части запроса FROM
    /// </summary>
    public partial class QFrom
    {
        public override string ToString()
        {
            return "From";
        }
    }

    public partial class QFromItem
    {
        public override string ToString()
        {
            return "Join: " + joined.ToString();
        }
    }

    /// <summary>
    /// Wrap the intermediate source field
    /// </summary>
    public partial class QIntermediateSourceField
    {
        public override string GetName()
        {
            return Field.GetName();
        }

        public override IEnumerable<SMType> GetExpressionType()
        {
            return Field.GetExpressionType();
        }

        public override string ToString()
        {
            return $"In-ate: {Field} From {DataSource}";
        }
    }

    /// <summary>
    /// Nested query field
    /// </summary>
    public partial class QNestedQueryField
    {
        public override string GetName()
        {
            return Field.GetName();
        }

        public override IEnumerable<SMType> GetExpressionType()
        {
            return Field.GetExpressionType();
        }

        public override string ToString()
        {
            return $"Nested: {Field} From {DataSource}";
        }
    }

    /// <summary>
    /// Вложенный запрос
    /// </summary>
    public partial class QNestedQuery : QDataSource
    {
        private List<QField> _fields;

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= Nested.Select.Fields.Select(x => (QField)new QNestedQueryField(x, this))
                .ToList();
        }

        public override string ToString()
        {
            return "Nested Query";
        }
    }

    /// <summary>
    /// Describes object table by the matadata
    /// </summary>
    public partial class QObjectTable
    {
        private List<QField> _fields;
        private List<QTable> _tables;

        public QObjectTable(SMEntity type)
        {
            ObjectType = type;
        }

        /// <summary>
        /// Ссылка на тип объекта
        /// </summary>
        public SMEntity ObjectType { get; }

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= ObjectType.Properties.Select(x => (QField)new QSourceFieldExpression(this, x))
                .ToList();
        }

        public override IEnumerable<QTable> GetTables()
        {
            throw new NotImplementedException();

            // return _tables ??= ObjectType.Tables.Select(x => new QTable(this, x))
            //     .ToList();
        }

        public override string ToString()
        {
            return "Object: " + ObjectType.Name;
        }
    }

    public partial class QTable
    {
        private List<QField> _fields;

        public string Name => null;

        public override IEnumerable<QField> GetFields()
        {
            return _fields ?? throw new NotImplementedException();
        }
    }

    public partial class QOperationExpression : QExpression
    {
        public override IEnumerable<SMType> GetExpressionType()
        {
            return Left.GetExpressionType();
        }
    }

    /// <summary>
    /// Представляет параметр
    /// </summary>
    public partial class QParameter : QExpression
    {
        private List<SMType> _types = new List<SMType>();

        public void AddType(SMType type)
        {
            if (!_types.Contains(type)) _types.Add(type);
        }

        public override IEnumerable<SMType> GetExpressionType()
        {
            return _types;
        }

        public override string ToString()
        {
            return $"Param {Name}";
        }
    }

    /// <summary>
    /// Заппрос
    /// </summary>
    public partial class QQuery
    {
        public override string ToString()
        {
            return "Query";
        }
    }

    public partial class QSelect
    {
        public override string ToString()
        {
            return "Select";
        }
    }

    partial class QWhere
    {
        public override string ToString()
        {
            return "Where";
        }
    }

    /// <summary>
    /// Выражение в предложении  SELECT 
    /// </summary>
    public partial class QSelectExpression : QField
    {
        public override IEnumerable<SMType> GetExpressionType()
        {
            return (Expression).GetExpressionType();
        }

        public override string GetName()
        {
            if (Expression is QField ase) return ase.GetName();
            return base.GetName();
        }

        public override string ToString()
        {
            return "Expr";
        }
    }

    /// <summary>
    /// Выражение, основанное на конечном поле данных
    /// </summary>
    public partial class QSourceFieldExpression : QField
    {
        public override string GetName()
        {
            return Property.Name;
        }

        public override IEnumerable<SMType> GetExpressionType()
        {
            return Property.Types;
        }

        public override string ToString()
        {
            return "Field: " + Property.Name;
        }
    }

    public partial class QLookupField : QField
    {
        public IEnumerable<SMProperty> GetProperties()
        {
            throw new NotImplementedException();

            // return BaseExpression.GetExpressionType().Where(x =>
            //         x.IsObject && x.Properties.Any(p => p.Name == PropName))
            //     .Select(x => x.FindPropertyByName(PropName));
        }

        public override IEnumerable<SMType> GetExpressionType()
        {
            return GetProperties().SelectMany(x => x.Types);
        }

        public override string GetName()
        {
            return PropName;
        }
    }

    /// <summary>
    /// Сумма
    /// </summary>
    public class QSum : QExpression
    {
        private readonly SMType _baseSMType;

        public QSum(SMType baseSMType)
        {
            _baseSMType = baseSMType;
        }

        public override IEnumerable<SMType> GetExpressionType()
        {
            yield return _baseSMType;
        }
    }

    public partial class QCombinedDataSource
    {
        private List<QField> _fields;

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= DataSources
                .SelectMany(x => x.GetFields())
                // .Select()
                .ToList();
        }
    }

    public partial class QVar
    {
        SMType _bType = new SMType(SMType.Unknown);

        public void BindType(SMType type)
        {
            _bType = type;
        }

        public override IEnumerable<SMType> GetExpressionType()
        {
            yield return _bType;
        }

        public override string ToString()
        {
            return $"Variable {Name}";
        }
    }
}