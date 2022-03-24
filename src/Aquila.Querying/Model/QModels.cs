using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Aquila.Metadata;

namespace Aquila.Core.Querying.Model
{
    public partial class QAliasedDataSource : QDataSource
    {
        private List<QField> _fields;

        public override bool HasInternalCriterion => ParentSource.HasInternalCriterion;

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

    public partial class QCount
    {
        public override IEnumerable<SMType> GetExpressionType()
        {
            yield return new SMType(SMType.Int);
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
        public bool Substituted { get; set; }

        public virtual IEnumerable<QField> GetFields()
        {
            return null;
        }

        public virtual QField GetField(string name) => GetFields().FirstOrDefault(x => x.GetName() == name);


        public virtual bool HasInternalCriterion => false;

        public virtual bool HasField(string name)
        {
            return GetFields().Any(x => x.GetName() == name);
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

        public bool IsComplexExprType => GetExpressionType().Count() > 1;


        /// <summary>
        /// Returns null if can't get source
        /// </summary>
        /// <returns></returns>
        public QDataSource GetSource()
        {
            QField result;

            if (this is QIntermediateSourceField isf1)
            {
                if (isf1.Field is QIntermediateSourceField isf)
                    result = isf.Field;
                else
                    result = isf1.Field;

                return isf1.DataSource;
            }

            return null;
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

        public override bool HasInternalCriterion => Nested.Criteria.Any();

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
    public partial class QObject
    {
        private List<QField> _fields;
        private List<QTable> _tables;

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= ObjectType.Properties.Select(x => (QField)new QSourceFieldExpression(this, x))
                .ToList();
        }

        public override IEnumerable<QTable> GetTables()
        {
            return _tables ??= ObjectType.Tables.Select(x => new QTable(this, x)).ToList();
        }

        public override string ToString()
        {
            return "Object: " + ObjectType.Name;
        }
    }

    public partial class QTable
    {
        private List<QField> _fields;

        public string Name => this.tableType.Name;

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= TableType.Properties.Select(x => (QField)new QSourceFieldExpression(this, x))
                .ToList();
        }

        public override string ToString()
        {
            return "Object: " + Name;
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
    public partial class QParameter
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

    public partial class QCriterion
    {
        public override string ToString()
        {
            return "Criterion";
        }
    }

    /// <summary>
    /// Заппрос
    /// </summary>
    public partial class QSelectQuery
    {
        public override string ToString()
        {
            return "Select Query";
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

    partial class QGroupBy
    {
        public override string ToString()
        {
            return "Group by";
        }
    }

    partial class QOrderBy
    {
        public override string ToString()
        {
            return "Order by";
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
            foreach (var type in BaseExpression.GetExpressionType())
            {
                if (type.IsReference)

                    foreach (var prop in type.GetSemantic().Properties)
                    {
                        if (prop.Name == PropName)
                            yield return prop;
                    }
            }
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

    public partial class QInsert
    {
        public override string ToString()
        {
            return "Insert";
        }
    }

    public partial class QInsertQuery
    {
        public override string ToString()
        {
            return "Insert query";
        }
    }


    public partial class QTypedParameter
    {
        public override IEnumerable<SMType> GetExpressionType()
        {
            return Types;
        }
    }
}