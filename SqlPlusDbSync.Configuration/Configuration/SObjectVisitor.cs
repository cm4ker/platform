using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using QueryCompiler;
using QueryCompiler.Schema;
using SqlPlusDbSync.Configuration.Configuration;
using SqlPlusDbSync.Data.Database;

namespace SqlPlusDbSync.Platform.Configuration
{
    public class SObjectVisitor : DbObjectsBaseVisitor<object>
    {
        private List<PType> _objects = new List<PType>();
        private List<SEvent> _events = new List<SEvent>();
        private List<SPoint> _points = new List<SPoint>();
        private List<SPoint> _ownerPoints = new List<SPoint>();
        private AsnaDatabaseContext _context;
        private DBSchemaManager schemaManager;


        public SObjectVisitor(AsnaDatabaseContext context)
        {
            _context = context;
            schemaManager = new DBSchemaManager(_context);
        }

        public string GetFullText(ParserRuleContext context)
        {
            if (context.Start == null || context.Stop == null || context.Start.StartIndex < 0 || context.Stop.StopIndex < 0)
                return context.GetText(); // Fallback

            return context.Start.InputStream.GetText(Interval.Of(context.Start.StartIndex, context.Stop.StopIndex));
        }

        public override object VisitObjectDefinitionTable(DbObjectsParser.ObjectDefinitionTableContext context)
        {
            _events = new List<SEvent>();
            var result = new TableType();

            result.Name = context.objectDefinitionName().GetText();
            var table = new STable() { Name = context.tableExpression().tableName().GetText() };

            result.IsTransfered = context.TRANSFER() != null;
            result.Table = table;

            if (context.tableExpression().includeExpression(0) != null)
            {
                foreach (var column in context.tableExpression().includeExpression(0).columnList())
                {
                    table.Fields.Add(VisitColumnList(column, result) as SField);
                }
            }
            else
            {
                var schema = schemaManager.GetTableSchema(table.Name);
                foreach (var dbField in schema)
                {
                    var fieldSchema = new SSchema(dbField.Type, dbField.ColumnName, dbField.ColumnSize, dbField.NumericPrecision, dbField.NumericScale, dbField.IsKey, dbField.IsUnique, dbField.IsNullable);
                    table.Fields.Add(new SField() { Name = dbField.ColumnName, Owner = result, Schema = fieldSchema });
                }
            }

            if (context.tableExpression().excludeExpression(0) != null)
            {
                foreach (var column in context.tableExpression().excludeExpression(0).columnList())
                {
                    var excludedField = VisitColumnList(column, result) as SField;
                    var field = table.Fields.Find(x => x.Name.ToLower() == excludedField.Name.ToLower());
                    if (field != null)
                    {
                        table.Fields.Remove(field);
                    }
                }
            }

            if (context.tableExpression().uniqueExpression() != null)
            {
                foreach (var column in context.tableExpression().uniqueExpression().columnList())
                {
                    var uniqueField = VisitColumnList(column, result) as SField;
                    var field = table.Fields.Find(x => x.Name.ToLower() == uniqueField.Name.ToLower());

                    if (field != null)
                    {
                        field.IsIdentifier = true;
                    }
                }
            }

            if (context.tableExpression().pointExpression(0) != null)
            {
                var pointName = context.tableExpression().pointExpression(0).pointName().GetText();
                var fieldName = context.tableExpression().pointExpression(0).fieldName().GetText();

                var point = _points.Find(x => x.Name.ToLower() == pointName.ToLower());
                if (point is null) throw new Exception($"Point definition {pointName} not found");

                var field = table.Fields.Find(x => x.Name.ToLower() == fieldName.ToLower());

                if (field is null) throw new Exception($"Point definition {fieldName} not found");

                field.Point = point;
            }

            if (context.tableExpression().ownerExpression(0) != null)
            {
                var ownerName = context.tableExpression().ownerExpression(0).ownerName().GetText();
                var fieldName = context.tableExpression().ownerExpression(0).fieldName().GetText();

                var ownerPoint = _ownerPoints.Find(x => x.Name.ToLower() == ownerName.ToLower());
                if (ownerPoint is null) throw new Exception($"Point definition {ownerName} not found");

                var field = table.Fields.Find(x => x.Name.ToLower() == fieldName.ToLower());

                if (field is null) throw new Exception($"Point definition {fieldName} not found");

                field.OwnerPoint = ownerPoint;
            }

            //foreach (var wExp in context.whereExpression())
            //{
            //    var c = new SCondition();
            //    result.Condition = c;
            //}

            if (context.objectEvents() != null)
                this.VisitChildren(context.objectEvents());

            foreach (var whereExp in context.whereExpression())
            {
                if (whereExp.compareOperatorExpression().EQUALS() != null)
                {
                    var operand = new SOperand(whereExp.valueExpression().NUMBER().GetText());
                    var sfield =
                        result.Fields.First(x => x.Name.ToLower() ==
                                                 whereExp.fieldExpression().fieldName().GetText().ToLower());

                    var con = new SWhereCondition(sfield, operand, CompareType.Equals);
                    result.WhereCondition = con;
                }
                if (whereExp.compareOperatorExpression().IN() != null)
                {
                    var arr = new List<object>();
                    foreach (var arg in whereExp.valueExpression().numberArrayExpression().NUMBER())
                    {
                        arr.Add(arg.GetText());
                    }
                    var operand = new SOperand(arr.ToArray());
                    var sfield =
                        result.Fields.First(x => x.Name.ToLower() ==
                                                 whereExp.fieldExpression().fieldName().GetText().ToLower());

                    var con = new SWhereCondition(sfield, operand, CompareType.In);
                    result.WhereCondition = con;
                }
            }

            result.Events.AddRange(_events);
            VisitOptions(context.objectOptions(), result);

            _objects.Add(result);

            return _objects;
        }

        private void SetDefaultOptions(PType obj)
        {
            obj.Direction = SDirection.Any;
        }

        public override object VisitPointDefinition(DbObjectsParser.PointDefinitionContext context)
        {
            var result = new SPoint();
            result.Name = context.pointName().GetText();
            result.From = new STable() { Name = context.tableName().GetText() };
            result.OnSField = new SField() { Name = context.onFieldName().GetText() };
            result.ValueSField = new SField() { Name = context.valueFieldName().GetText() };
            _points.Add(result);
            return result;
        }

        public override object VisitOwnerDefinition(DbObjectsParser.OwnerDefinitionContext context)
        {
            var result = new SPoint();
            result.Name = context.ownerName().GetText();
            result.From = new STable() { Name = context.tableName().GetText() };
            result.OnSField = new SField() { Name = context.onFieldName().GetText() };
            result.ValueSField = new SField() { Name = context.valueFieldName().GetText() };
            _ownerPoints.Add(result);
            return result;
        }

        public SField VisitColumnList(DbObjectsParser.ColumnListContext context, PType owner)
        {
            var field = VisitColumnList(context) as SField;
            field.Owner = owner;
            return field;
        }

        public override object VisitColumnList(DbObjectsParser.ColumnListContext context)
        {
            var result = new SField();
            result.Name = context.ID().GetText();
            return result;
        }

        private TableType GetParentTableObject(PType obj)
        {
            if (obj is TableType) return (TableType)obj;
            return GetParentTableObject(obj.Source);
        }

        private PType GetObject(string objectName)
        {
            var result = _objects.FirstOrDefault(x => x.Name == objectName);
            if (result == null) throw new ObjectNotfound(objectName);

            return result;
        }

        public override object VisitObjectDefinitionObject(DbObjectsParser.ObjectDefinitionObjectContext context)
        {
            _events = new List<SEvent>();

            var result = new TypeType();
            result.Name = context.objectDefinitionName().GetText();

            var pContext = context.objectPoints();
            if (context.objectPoints() != null)
            {
                var pointName = pContext.pointName().GetText();
                var sObjectName = pContext.fieldExpression().tableName().GetText();
                var sFieldName = pContext.fieldExpression().fieldName().GetText();

                var spoint = _points.Find(x => x.Name.ToLower() == pointName.ToLower());

                if (spoint == null)
                {
                    throw new Exception($"Point with name {pointName} not found");
                }

                var sObject = GetObject(sObjectName);
                var sField = sObject.Fields.Find(x => x.Name.ToLower() == sFieldName.ToLower());

                foreach (var field in sObject.Fields.Where(x => x.Point != null))
                {
                    field.Point = null;
                }

                if (sField != null)
                    sField.Point = spoint;
            }

            var parentObject = GetObject(context.objectName().GetText());
            foreach (var field in parentObject.Fields)
            {
                result.Fields.Add(field.GetCopyFieldWithOwner(result));
            }

            result.Source = parentObject;
            // result.Condition = parentObject.Condition;
            result.IsTransfered = context.TRANSFER() != null;
            _objects.Add(result);
            foreach (var rel in context.objectRelations().objectRelation())
            {
                var relObject = _objects.First(x => x.Name == rel.ID().GetText());
                List<SCondition> conditions = new List<SCondition>();
                foreach (var jc in rel.@join())
                {
                    var fe1 = jc.fieldExpression(0);
                    var fe2 = jc.fieldExpression(1);

                    var fe1ObjectName = fe1.tableName().ID().GetText();
                    var fe1FieldName = fe1.fieldName().ID().GetText();

                    var fe2ObjectName = fe2.tableName().ID().GetText();
                    var fe2FieldName = fe2.fieldName().ID().GetText();

                    var o1 = GetObject(fe1ObjectName);
                    var o2 = GetObject(fe2ObjectName);
                    conditions.Add(new SCondition(o1, fe1FieldName, o2, fe2FieldName));
                }

                SRelationType relType;

                switch (rel.REL_TYPE().ToString())
                {
                    case "ONE_TO_MANY": relType = SRelationType.OneToMany; break;
                    case "ONE_TO_ONE": relType = SRelationType.OneToOne; break;
                    default: throw new Exception("Unsupported relation type");
                }


                result.Relations.Add(new SRelation() { Type = relObject, Condition = conditions, RelationType = relType });
            }

            if (context.objectEvents() != null)
                this.VisitChildren(context.objectEvents());

            var options = context.objectOptions();

            VisitOptions(options, result);

            result.Events.AddRange(_events);
            return _objects;
        }

        public void VisitOptions(DbObjectsParser.ObjectOptionsContext context, PType owner)
        {
            if (context is null)
            {
                SetDefaultOptions(owner);
                return;
            }

            var direction = context.directionOptions();

            if (direction != null)
            {
                if (direction.UP() is null && direction.DOWN() is null)
                {
                    owner.Direction = SDirection.Any;
                }
                else if (direction.UP() is null)
                {
                    owner.Direction = SDirection.Down;
                }
                else
                {
                    owner.Direction = SDirection.Up;
                }

            }
        }

        public void RegisterEvent(ParserRuleContext eventContext)
        {
            var contexType = eventContext.GetType();
            var createMethod = contexType.GetMethod("CREATE", new Type[] { });
            var updateMethod = contexType.GetMethod("UPDATE", new Type[] { });
            var deleteMethod = contexType.GetMethod("DELETE", new Type[] { });
            var errorMethod = contexType.GetMethod("ERROR", new Type[] { });

            var triggerMethod = (createMethod ?? updateMethod ?? deleteMethod ?? errorMethod).Name.ToLower();
            var eventTriggerEvalType = contexType.Name.Contains("after") ? "after" : "on";

            var method = contexType.GetMethods().Single(x => x.Name.ToLower() == $"{eventTriggerEvalType}{triggerMethod}statement");
            var concreteEventContext = method.Invoke(eventContext, null);
            if (concreteEventContext != null)
            {
                var invokeMethodStatement = concreteEventContext.GetType().GetMethod("invokeStatement", new Type[] { });
                var updateAllReferencesMethodStatement = concreteEventContext.GetType().GetMethods()
                    .FirstOrDefault(x => x.Name == "updateAllReferencesStatement" && x.GetParameters().Length == 0);
                var deleteCascadeMethodStatement = concreteEventContext.GetType().GetMethods()
                    .FirstOrDefault(x => x.Name == "deleteCascadeStatement" && x.GetParameters().Length == 0);



                DbObjectsParser.InvokeStatementContext[] invokeStatementContext =
                    invokeMethodStatement.Invoke(eventContext,
                        null) as DbObjectsParser.InvokeStatementContext[];
            }
        }

        public override object VisitObjectOnEvent(DbObjectsParser.ObjectOnEventContext context)
        {
            return this.VisitChildren(context);
        }

        public override object VisitInvokeStatement(DbObjectsParser.InvokeStatementContext context)
        {
            return base.VisitInvokeStatement(context);
        }

        public override object VisitObjectEvents(DbObjectsParser.ObjectEventsContext context)
        {
            if (context == null) return null;
            return base.VisitObjectEvents(context);
        }

        public override object VisitInvokeBodyStatement(DbObjectsParser.InvokeBodyStatementContext context)
        {
            _events.Add(new SEvent() { Name = context.parent.parent.GetType().Name.Replace("StatementContext", ""), Body = GetFullText(context), EventType = EventType.Invoke });
            //Console.WriteLine("{0}, {2} ,{1}", context.parent.parent.GetType().Name.Replace("StatementContext", ""), GetFullText(context), "INVOKE");
            return this.VisitChildren(context);
        }

        public override object VisitUpdateAllReferencesStatement(DbObjectsParser.UpdateAllReferencesStatementContext context)
        {
            _events.Add(new SEvent() { Name = context.parent.GetType().Name.Replace("StatementContext", ""), Body = GetFullText(context), EventType = EventType.UpdateAllRef });
            //Console.WriteLine("{0}, {2} ,{1}", context.parent.GetType().Name.Replace("StatementContext", ""), GetFullText(context), "ALL_REF_STTM");
            return base.VisitUpdateAllReferencesStatement(context);
        }

        public override object VisitDeleteCascadeStatement(DbObjectsParser.DeleteCascadeStatementContext context)
        {
            _events.Add(new SEvent() { Name = context.parent.GetType().Name.Replace("StatementContext", ""), Body = GetFullText(context), EventType = EventType.CascadeDelete });
            //Console.WriteLine("{0}, {2} ,{1}", context.parent.GetType().Name.Replace("StatementContext", ""), GetFullText(context), "DEL_CASCADE");
            return base.VisitDeleteCascadeStatement(context);
        }

    }

    public class SPoint
    {
        public string Name { get; set; }
        public SField OnSField { get; set; }
        public SField ValueSField { get; set; }
        public STable From { get; set; }
    }
}