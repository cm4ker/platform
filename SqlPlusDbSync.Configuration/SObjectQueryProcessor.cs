//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Xml;
//using FluentMigrator.Runner.Versioning;
//using QueryCompiler;
//using QueryCompiler.Queries;
//using SqlPlusDbSync.Configuration.Configuration;
//using SqlPlusDbSync.Data.Database;
//using SqlPlusDbSync.Platform.EntityObject;
//using IQueryable = QueryCompiler.Queries.IQueryable;

//namespace SqlPlusDbSync.Platform.Configuration
//{
//    public class SObjectQueryProcessor : IDisposable
//    {
//        private readonly AsnaDatabaseContext _context;
//        private DBQueryCompiler _qc;
//        private Dictionary<Tuple<string, Type>, IQueryable> _cache;

//        public SObjectQueryProcessor(AsnaDatabaseContext context)
//        {
//            _context = context;
//            _qc = new DBQueryCompiler(_context);
//            _cache = new Dictionary<Tuple<string, Type>, IQueryable>();
//        }

//        public DBQueryCompiler QueryCompiler => _qc;

//        public DBUpdateQuery GetTableObjectUpdateQuery(TableType sType)
//        {
//            var qc = new DBQueryCompiler(_context);

//            var updateQuery = qc.CreateUpdate();

//            var table = sType.GetDBTable(qc);
//            var identitySObjectField = sType.Fields.Single(x => x.IsIdentifier);
//            var identityField = table.Fields.Single(x => (x as DBTableField).Name == identitySObjectField.Name) as DBTableField;
//            updateQuery.AddFrom(table);

//            foreach (var field in table.Fields)
//            {
//                if ((field as DBTableField).Schema.IsKey) continue;
//                updateQuery.AddField(field as DBTableField);
//            }

//            updateQuery.Where(identityField, CompareType.Equals, DBClause.CreateParameter(PlatformHelper.IdentityParameter + DBHelper.GetRandomString(12), identityField.Schema.Type));
//            return updateQuery;
//        }

//        public DBDeleteQuery GetTableObjectDeleteQuery(TableType sType)
//        {

//            var deleteQuery = _qc.CreateDelete();

//            var table = sType.GetDBTable(_qc);
//            var identitySObjectField = sType.Fields.Single(x => x.IsIdentifier);
//            var identityField = table.Fields.Single(x => (x as DBTableField).Name == identitySObjectField.Name) as DBTableField;

//            deleteQuery.AddFrom(table);
//            deleteQuery.DeleteTable = table;
//            deleteQuery.Where(identityField, CompareType.Equals, DBClause.CreateParameter(PlatformHelper.IdentityParameter + DBHelper.GetRandomString(12), identityField.Schema.Type));

//            return deleteQuery;
//        }

//        public DBInsertQuery GetTableObjectInsertQuery(TableType sType)
//        {
//            var insertQuery = _qc.CreateInsert();

//            var table = sType.GetDBTable(_qc);

//            foreach (var field in table.Fields)
//            {
//                insertQuery.AddField(field as DBTableField);
//            }

//            return insertQuery;
//        }

//        public DBBatch GetDeleteQuery(DTOObject dtoObject, PType pType)
//        {
//            var result = new DBBatch();
//            var entityType = dtoObject.GetType();

//            if (!dtoObject.Register)
//            {
//                result.AddQuery(new DBSetVariableClause(new DBVariable(SQLVariables.CONTEXT_INFO), new DBHexConstant(0x1234)));
//            }

//            if (!(pType is TableType))
//                foreach (var rel in pType.Relations)
//                {
//                    var prop = entityType.GetProperty(rel.Type.Name);
//                    if (prop is null) throw new Exception($"Property name {rel.Type.Name} not found");

//                    var relCollection = prop.GetValue(dtoObject, null) as IList;

//                    foreach (DTOObject relEntity in relCollection)
//                    {
//                        result.AddBatch(GetDeleteQuery(relEntity, rel.Type));
//                    }
//                }

//            var to = pType.GetTableObject();

//            var deleteQuery = GetTableObjectDeleteQuery(to);
//            deleteQuery.Parameters[0].SetValue(dtoObject.Key);
//            result.AddQuery(deleteQuery);

//            if (!dtoObject.Register)
//            {
//                result.AddQuery(new DBSetVariableClause(new DBVariable(SQLVariables.CONTEXT_INFO), new DBHexConstant(null)));
//            }

//            return result;
//        }

//        public DBBatch GetSaveQuery(DTOObject dtoObject, PType pType, SaveAction action = SaveAction.None)
//        {
//            if (action == SaveAction.None)
//                action = dtoObject.DynamicProperties.Action;

//            var result = new DBBatch();
//            var entityType = dtoObject.GetType();

//            if (!dtoObject.Register)
//            {
//                result.AddQuery(new DBSetVariableClause(new DBVariable(SQLVariables.CONTEXT_INFO), new DBHexConstant(0x1234)));
//            }

//            if (!(pType is TableType))
//                foreach (var rel in pType.Relations)
//                {
//                    var prop = entityType.GetProperty(rel.Type.Name);
//                    if (prop is null) throw new Exception($"Property name {rel.Type.Name} not found");

//                    var relCollection = prop.GetValue(dtoObject, null) as IList;

//                    foreach (DTOObject relEntity in relCollection)
//                    {
//                        result.AddBatch(GetSaveQuery(relEntity, rel.Type, relEntity.DynamicProperties.Action));
//                    }
//                }

//            var to = pType.GetTableObject();

//            if (action == SaveAction.Update)
//            {
//                var updateQuery = GetTableObjectUpdateQuery(to);

//                int paramIndex = 0;

//                foreach (var field in pType.Fields)
//                {
//                    var prop = entityType.GetProperty(field.Name);
//                    if (prop is null) throw new Exception($"Property name {field.Name} not found");

//                    var fieldParamName = updateQuery.Parameters[paramIndex].Name.TrimStart('@');
//                    if (field.Name.ToLower() == fieldParamName
//                            .Substring(0, fieldParamName.Length - (DBHelper.RandomCharsInParams() + 1)).ToLower())
//                    {
//                        updateQuery.Parameters[paramIndex].SetValue(prop.GetValue(dtoObject, null));
//                        paramIndex++;
//                    }
//                }

//                if (paramIndex != updateQuery.Parameters.Count - 1) throw new Exception("Platform not map all parameters then generating save query");

//                updateQuery.Parameters.Last().SetValue(dtoObject.Key);
//                result.AddQuery(updateQuery);
//            }

//            if (action == SaveAction.Delete)
//            {
//                result.AddBatch(GetDeleteQuery(dtoObject, pType));
//            }

//            if (action == SaveAction.Insert)
//            {
//                var insertQuery = GetTableObjectInsertQuery(to);

//                for (int i = 0; i < pType.Fields.Count; i++)
//                {
//                    var prop = entityType.GetProperty(pType.Fields[i].Name);
//                    if (prop is null) throw new Exception($"Property name {pType.Fields[i].Name} not found");

//                    insertQuery.Parameters[i].SetValue(prop.GetValue(dtoObject, null));
//                }
//                if (insertQuery.Fields.Any(x => x.Schema.IsIdentity))
//                {
//                    result.AddQuery(_qc.CreateSetIdentity(to.Table.Name, true));
//                    result.AddQuery(insertQuery);
//                    result.AddQuery(_qc.CreateSetIdentity(to.Table.Name, false));
//                }
//                else
//                {
//                    result.AddQuery(insertQuery);
//                }

//            }


//            if (!dtoObject.Register)
//            {
//                result.AddQuery(new DBSetVariableClause(new DBVariable(SQLVariables.CONTEXT_INFO), new DBHexConstant(null)));
//            }

//            return result;
//        }

//        public DBSelectQuery GetSelectQuery(PType sobject, int depth = 0)
//        {
//            if (_cache.TryGetValue(new Tuple<string, Type>(sobject.Name, typeof(DBSelectQuery)), out var query))
//            {
//                return (query as DBSelectQuery)?.Clone() as DBSelectQuery;
//            }

//            var selectQuery = _qc.CreateSelect();

//            if (sobject is TableType)
//            {
//                TableType to = sobject as TableType;

//                var table = _qc.CreateTable(to.Table.Name, to.GetFullName());

//                foreach (var field in to.Fields)
//                {
//                    table.DeclareField(field.Name);
//                }
//                selectQuery.From(table);
//                selectQuery.SelectAllFieldsFromSourceTables(to.GetFullName() + '.');
//            }
//            else
//            {
//                var parentQuery = GetSelectQuery(sobject.Source, depth + 1);
//                var subQuery = parentQuery.GetAsSubQuery(sobject.Name);
//                selectQuery.From(subQuery);

//                foreach (var sobjectRelation in sobject.Relations)
//                {
//                    var relQuery = GetSelectQuery(sobjectRelation.Type, depth + 1).GetAsSubQuery(sobjectRelation.Type.Name);

//                    foreach (var con in sobjectRelation.Condition)
//                    {
//                        var f1 = subQuery.GetField(con.Field1.GetFullName());
//                        var f2 = relQuery.GetField(con.Field2.GetFullName());

//                        selectQuery.Join(relQuery, JoinType.Left).On(f1, CompareType.Equals, f2);
//                    }
//                }
//                selectQuery.SelectAllFieldsFromSourceTables(sobject.Name + ".");
//            }
//            if (depth == 0)
//                _cache[new Tuple<string, Type>(sobject.Name, typeof(DBSelectQuery))] = selectQuery.Clone() as IQueryable;
//            return selectQuery.Clone() as DBSelectQuery;
//        }

//        public DBSelectQuery GetSelectGraphQuery(PType sobject, int depth = 0)
//        {
//            var selectQuery = _qc.CreateSelect();

//            if (sobject is TableType)
//            {
//                TableType to = sobject as TableType;

//                var table = _qc.CreateTable(to.Table.Name, to.GetFullName());
//                foreach (var field in to.Fields)
//                {
//                    table.DeclareField(field.Name);
//                }
//                selectQuery.From(table);
//                selectQuery.SelectAllFieldsFromSourceTables(to.GetFullName() + '.');
//            }
//            else
//            {
//                var parentQuery = GetSelectGraphQuery(sobject.Source, depth + 1);
//                var subQuery = parentQuery.GetAsSubQuery(sobject.Name);
//                selectQuery.From(subQuery);

//                foreach (var sobjectRelation in sobject.Relations)
//                {
//                    var relQuery = GetSelectGraphQuery(sobjectRelation.Type, depth + 1).GetAsSubQuery(sobjectRelation.Type.Name);

//                    foreach (var con in sobjectRelation.Condition)
//                    {
//                        var f1 = subQuery.GetField(con.Field1.GetFullName());
//                        var f2 = relQuery.GetField(con.Field2.GetFullName());

//                        selectQuery.Join(relQuery, JoinType.Left).On(f1, CompareType.Equals, f2);
//                    }
//                }
//                selectQuery.SelectAllFieldsFromSourceTables(sobject.Name + ".");
//            }

//            if (depth == 0)
//            {
//                var fields = GetIdentityFields(sobject);
//                var selectedFields = selectQuery.Fields.ToArray();
//                foreach (DBSelectField selectField in selectedFields)
//                {
//                    var delete = true;
//                    foreach (var field in fields)
//                    {
//                        if (selectField.Alias.Contains(field.ColumnName))
//                        {
//                            delete = false;
//                        }
//                    }
//                    if (delete)
//                        selectQuery.Fields.Remove(selectField);
//                }
//            }


//            return selectQuery;
//        }

//        private List<PProperty> GetIdentityFields(PType sobject)
//        {
//            var result = new List<PProperty>();
//            if (sobject is TableType) result.Add(sobject.GetIdentity());
//            else
//            {
//                result.AddRange(GetIdentityFields(sobject.GetTableObject()));
//                foreach (var relation in sobject.Relations)
//                {
//                    result.AddRange(GetIdentityFields(relation.Type));
//                }
//            }

//            return result;
//        }

//        public DBSelectQuery GetSingleSelectQuery(PType sobject)
//        {
//            var selectQuery = GetSelectQuery(sobject);

//            var identity = sobject.GetIdentity();
//            var field = selectQuery.GetField(sobject.GetFullName() + '.' + identity.Name) as DBField;

//            selectQuery.Where(field, CompareType.Equals, DBClause.CreateParameter(PlatformHelper.IdentityParameter, field.Schema.Type));
//            return selectQuery;
//        }

//        public DBSelectQuery GetSingleSelectGraphQuery(PType sobject)
//        {
//            var selectQuery = GetSelectGraphQuery(sobject);

//            var identity = sobject.GetIdentity();
//            var field = selectQuery.GetField(sobject.GetFullName() + '.' + identity.Name) as DBField;

//            selectQuery.Where(field, CompareType.Equals, DBClause.CreateParameter(PlatformHelper.IdentityParameter, field.Schema.Type));
//            return selectQuery;
//        }

//        public DBSelectQuery GetObjectOwnerQuery(PType pType)
//        {
//            var sFieldOwnerPoint = pType.Fields.Find(x => x.OwnerPoint != null);
//            if (sFieldOwnerPoint != null)
//            {
//                var objectTable = pType.GetTableObject().GetDBTable(_qc);
//                var ownerPoint = sFieldOwnerPoint.OwnerPoint;
//                var ownerTable = _qc.CreateTable(ownerPoint.From.Name, "OwnerPointInfo");
//                ownerTable.FillFieldsFromSchema();

//                var identy = pType.GetIdentity();
//                var identityField = objectTable.GetField(identy.Name) as DBTableField;

//                var sq = _qc.CreateSelect().Top(1);
//                sq.From(objectTable)
//                    .Join(ownerTable, JoinType.Left)
//                    .On(ownerTable.GetField(ownerPoint.OnSField.Name), CompareType.Equals, objectTable.GetField(sFieldOwnerPoint.Name));

//                sq.Where(identityField, CompareType.Equals, DBClause.CreateParameter(PlatformHelper.IdentityParameter, identityField.Schema.Type));

//                sq.Select(ownerTable.GetField(ownerPoint.ValueSField.Name) as DBField);

//                return sq;
//            }
//            return null;
//        }

//        public DBSelectQuery GetPackageQuery(PType pType)
//        {
//            var to = pType.GetTableObject();

//            DBSelectPipe pipe = new DBSelectPipe();

//            DBSelectQuery sq = _qc.CreateSelect();

//            var table = to.GetDBTable(_qc);
//            var versionTable = PlatformHelper.GetVersionTable(_qc);
//            var metadataTable = PlatformHelper.GetMetadataTable(_qc);

//            sq.From(versionTable);

//            sq.Join(table, JoinType.Left)
//                .AndOn(table.GetField(to.GetIdentity().Name), CompareType.Equals, versionTable.GetField("id"));

//            DBLogicalOperation whereClause = sq.Where(versionTable.GetField("Version"), CompareType.GreatThen, DBClause.CreateParameter("@MinVersion", SqlDbType.VarBinary));

//            whereClause.AndAnd(versionTable.GetField("TableName"), CompareType.Equals, DBClause.CreateConstant(to.Table.Name));

//            if (to.WhereCondition != null)
//            {
//                whereClause.AndAnd(table.GetField(to.WhereCondition.Field.Name), to.WhereCondition.CompareType,
//                    DBClause.CreateConstant(to.WhereCondition.Operand.Value));
//            }
            
//            var sFieldPoint = pType.Fields.Find(x => x.Point != null);
//            if (sFieldPoint != null)
//            {
//                var pointInfo = sFieldPoint.Point;

//                var pointTable = _qc.CreateTable(pointInfo.From.Name, "PointInfo");
//                pointTable.FillFieldsFromSchema();

//                sq.Join(pointTable, JoinType.Left)
//                  .AndOn(pointTable.GetField(pointInfo.OnSField.Name), CompareType.Equals, table.GetField(sFieldPoint.Name));

//                whereClause.AndAnd(pointTable.GetField(pointInfo.ValueSField.Name), CompareType.Equals,
//                      DBClause.CreateParameter("@PointId", SqlDbType.UniqueIdentifier));
//            }

//            sq.Select(versionTable.GetField("Id"));
//            sq.Select(versionTable.GetField("Version"));
//            sq.Select(versionTable.GetField("TableName"));

//            var msq = sq.Clone() as DBSelectQuery;

//            sq.Select(new DBSelectConstantField(null, "Metadata"));

//            msq.Join(metadataTable, JoinType.Left)
//                    .AndOn(versionTable.GetField("id"), CompareType.Equals, metadataTable.GetField("id"));

//            msq.ClearWhere();
//            msq.Where(versionTable.GetField("Version"), CompareType.GreatThen, DBClause.CreateParameter("@MinVersion", SqlDbType.VarBinary));
//            msq.WhereNot(metadataTable.GetField("Metadata"), CompareType.IsNull, null);
//            msq.Select(metadataTable.GetField("Metadata"));

//            //msq.OrderBy(versionTable.GetField("Version"));

//            pipe.UnionAll(sq);
//            pipe.UnionAll(msq);

//            var newselect = new DBSelectQuery();
//            var versions = pipe.AsSubQuery("Versions");
//            newselect.From(versions);
//            newselect.SelectAllFieldsFromSourceTables();

//            newselect.OrderBy(versions.GetField("Version"));

//            return newselect;
//        }
//        public DBSelectQuery GetSelectFromXml(PType sobject, string xml)
//        {
//            var to = sobject.GetTableObject();
//            DBSelectQuery sq = _qc.CreateSelect();

//            XmlDocument xdoc = new XmlDocument();
//            xdoc.LoadXml(xml);

//            var xmlObject = xdoc.FirstChild;
//            var dataSelect = _qc.CreateSelect();

//            foreach (XmlNode prop in xmlObject.ChildNodes)
//            {
//                dataSelect.Select(new DBSelectConstantField(prop.InnerText, prop.Name));
//            }

//            var table = dataSelect.GetAsSubQuery("Object");
//            var versionTable = PlatformHelper.GetVersionTable(_qc);

//            sq.From(table);

//            sq.Join(versionTable).On(versionTable.GetField("Id"), CompareType.Equals, table.GetField(to.GetIdentity().Name));

//            if (to.WhereCondition != null)
//            {
//                sq.Where(table.GetField(to.WhereCondition.Field.Name), to.WhereCondition.CompareType,
//                    DBClause.CreateConstant(to.WhereCondition.Operand.Value));
//            }

//            sq.Where(versionTable.GetField("Version"), CompareType.GreatThen, DBClause.CreateParameter("@MinVersion", SqlDbType.VarBinary));

//            var sFieldPoint = sobject.Fields.Find(x => x.Point != null);
//            if (sFieldPoint != null)
//            {
//                var pointInfo = sFieldPoint.Point;

//                var pointTable = _qc.CreateTable(pointInfo.From.Name, "PointInfo");
//                pointTable.FillFieldsFromSchema();

//                sq.Join(pointTable, JoinType.Left)
//                    .AndOn(pointTable.GetField(pointInfo.OnSField.Name), CompareType.Equals,
//                        table.GetField(sFieldPoint.Name));

//                sq.Where(pointTable.GetField(pointInfo.ValueSField.Name), CompareType.Equals,
//                    DBClause.CreateParameter("@PointId", SqlDbType.UniqueIdentifier));
//            }

//            sq.Select(DBClause.CreateConstant("TRUE"));

//            return sq;
//        }

//        public void Dispose()
//        {
//            _cache = null;
//            _qc = null;
//        }
//    }
//}