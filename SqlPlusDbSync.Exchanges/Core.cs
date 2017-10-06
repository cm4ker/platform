using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;
using SqlPlusDbSync.Platform.Configuration;
using SqlPlusDbSync.Platform.EntityObject;
using SqlPlusDbSync.Platform.Syntax;
using SqlPlusDbSync.Shared;
using Environment = SqlPlusDbSync.Platform.EntityObject.Environment;


namespace SqlPlusDbSync.Exchanges
{
    public class Core
    {
        private List<SType> _supportedObjects;
        private readonly List<SObjectEntityManager> _managers;
        private readonly AsnaDatabaseContext _context;

        public Core(AsnaDatabaseContext context, bool handleMode = false)
        {
            _context = context;
            _supportedObjects = new List<SType>();
            _managers = new List<SObjectEntityManager>();

            PlatformInitializer pi = new PlatformInitializer();
            pi.Init(_context);

            if (!handleMode)
                LoadRules();
        }

        internal class SFieldComparer : IEqualityComparer<SField>
        {
            public bool Equals(SField x, SField y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(SField obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        public void Init()
        {
            Logger.LogInfo("Start initializing");

            PlatformInitializer pi = new PlatformInitializer();

            Logger.LogInfo("Start register and metadata triggers");
            foreach (var sobj in _supportedObjects.Where(x => x.IsTransfered))
            {
                var identityField = sobj.GetIdentity();
                var rootTable = sobj.GetTableObject();
                pi.RegisterTrigger(rootTable.Table.Name, identityField.Name, _context);
                pi.MetadataTrigger(rootTable.Table.Name, identityField.Name, _context);
            }
        }

        public void LoadRulesFromString(string text)
        {
            Logger.LogInfo("Loading rules");

            if (string.IsNullOrEmpty(text)) return;

            AntlrInputStream inputStream = new AntlrInputStream(text.ToString());
            DbObjectsLexer dbObjectsLexer = new DbObjectsLexer(inputStream);
            CommonTokenStream tokenStream = new CommonTokenStream(dbObjectsLexer);
            DbObjectsParser dboParser = new DbObjectsParser(tokenStream);
            SObjectVisitor visitor = new SObjectVisitor(_context);
            var listener = new ErrorListener();

            dboParser.AddErrorListener(listener);
            Logger.LogInfo("Start parse rules");
            _supportedObjects = visitor.Visit(dboParser.eval()) as List<SType>;
            Logger.LogInfo("End parse rules");

            Logger.LogInfo("Init entity managers...");
            foreach (var sobject in _supportedObjects)
            {
                _managers.Add(new SObjectEntityManager(_context, sobject));
            }
        }

        public void LoadRulesFromFile(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                SaveRules(sr.ReadToEnd());
                LoadRules();
            }
        }

        public string GetRules()
        {
            Logger.LogDebug("Starting get rules from database");
            return (string)_context.CreateCommand("SELECT RulesText FROM __Rules").ExecuteScalar() ?? "";
        }

        public void LoadRules()
        {
            LoadRulesFromString(GetRules());
        }

        public void SaveRules(string rules)
        {
            _context.BeginTransaction(IsolationLevel.Serializable);
            var cmd = _context.CreateCommand("DELETE FROM __Rules; INSERT INTO __Rules(RulesText) VALUES(@rulesText)");
            cmd.Parameters.Add(new SqlParameter("@rulesText", rules));
            cmd.ExecuteNonQuery();
            _context.CommitTransaction();

            LoadRules();
            Init();
        }

        public List<SType> SupportedObjects
        {
            get { return _supportedObjects; }
        }

        private bool CanBeTransferedFromCurrentPoint(SType sType)
        {
            if (sType.Direction == SDirection.Any) return true;

            if (_context.IsServer && sType.Direction == SDirection.Down) return true;

            if (!_context.IsServer && sType.Direction == SDirection.Up) return true;

            return false;
        }

        public List<ObjectResults> GetChangedIdFromVersion(byte[] lastVersion, Guid pointId)
        {
            List<ObjectResults> result = new List<ObjectResults>();

            SObjectQueryProcessor proc = new SObjectQueryProcessor(_context);

            foreach (var sObject in _supportedObjects.Where(x => x.IsTransfered))
            {
                if (!CanBeTransferedFromCurrentPoint(sObject)) continue;
                var sq = proc.GetPackageQuery(sObject);

                sq.Parameters.SetValueIfExists("@PointId", pointId);

                var uncommitedVersion = _context.GetLastUncommitedVersion(pointId, Config.Instance.MaxUncommitedTimeInSeconds);

                if (uncommitedVersion is null)
                    sq.Parameters["@MinVersion"].SetValue(lastVersion);
                else
                    sq.Parameters["@MinVersion"].SetValue(uncommitedVersion);

                var deletedObjects = new List<Tuple<SType, Guid, byte[], string, string>>();

                using (var cmd = _context.Connection.CreateCommand())
                {
                    cmd.CommandText = sq.Compile();

                    foreach (var sqParameter in sq.Parameters)
                    {
                        cmd.Parameters.Add(sqParameter.SqlParameter);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.GetSqlXml(3).IsNull)
                            {
                                //sobject, id, version, xml represent, table name
                                deletedObjects.Add(new Tuple<SType, Guid, byte[], string, string>(sObject, reader.GetGuid(0), (byte[])reader.GetValue(1), reader.GetSqlXml(3).Value, reader.GetString(2)));
                            }
                            else
                            {
                                result.Add(new ObjectResults(sObject, reader.GetGuid(0), (byte[])reader.GetValue(1)));
                            }
                        }
                    }

                }

                foreach (var item in deletedObjects)
                {
                    if (item.Item5.ToLower() != sObject.GetTableObject().Table.Name.ToLower()) continue;

                    var xmlQuery = proc.GetSelectFromXml(sObject, item.Item4);
                    xmlQuery.Parameters.SetValueIfExists("@PointId", pointId);

                    if (uncommitedVersion is null)
                        xmlQuery.Parameters["@MinVersion"].SetValue(lastVersion);
                    else
                        xmlQuery.Parameters["@MinVersion"].SetValue(uncommitedVersion);

                    using (var cmdXml = _context.Connection.CreateCommand())
                    {
                        cmdXml.CommandText = xmlQuery.Compile();

                        foreach (var sqParameter in xmlQuery.Parameters)
                        {
                            cmdXml.Parameters.Add(sqParameter.SqlParameter);
                        }

                        using (var xmlReader = cmdXml.ExecuteReader())
                        {
                            if (xmlReader.HasRows)
                                result.Add(new ObjectResults(sObject, item.Item2, item.Item3));
                        }
                    }
                }
            }

            var maxVersion = result.LastOrDefault()?.Version;
            if (maxVersion != null)
                _context.SaveLastUncommitedVersion(maxVersion, pointId);

            Logger.LogDebug($"Objects, changed from last request {result.Count}");
            return result;
        }

        internal class ByteArrayComparer : IComparer<byte[]>
        {
            public int Compare(byte[] x, byte[] y)
            {
                return PlatformHelper.ByteArrayCompare(x, y);
            }
        }

        public List<DTOObject> GetChangedEntityes(byte[] version, List<Guid> pointsId)
        {
            var entities = new List<DTOObject>();
            foreach (var pointId in pointsId)
            {
                var result = GetChangedIdFromVersion(version, pointId);

                foreach (var item in result)
                {
                    var entity = GetObject(item.SType, item.Id);
                    if (entity == null) continue;
                    if (entity.DynamicProperties.Cancel) continue;

                    entity.Version = item.Version;

                    entities.Add(entity);
                }
            }

            return entities.OrderBy(x => x.Version as byte[], new ByteArrayComparer()).ToList();

        }
        public List<DTOObject> GetDeletedEntityes(byte[] version, List<Guid> pointsId)
        {
            var entities = new List<DTOObject>();

            foreach (var pointId in pointsId)
            {
                var result = GetChangedIdFromVersion(version, pointId);

                foreach (var item in result)
                {
                    var entity = GetObject(item.SType, item.Id);
                    if (entity.DynamicProperties.Cancel) continue;
                    entity.Version = item.Version;
                    entity.Key = item.Id;

                    entities.Add(entity);
                }
            }

            return entities;
        }

        public DTOObject GetObject(SType sType, object id)
        {
            var mrg = GetEntityManager(sType);
            return mrg.Load(id);
        }
        private SObjectEntityManager GetEntityManager(DTOObject entiity)
        {
            var entityType = entiity.GetType();
            return _managers.Find(x => x.SType.Name == entityType.Name);
        }
        private SObjectEntityManager GetEntityManager(SType sobject)
        {
            var result = _managers.Find(x => x.SType.Equals(sobject));
            if (result is null) throw new Exception("Object not supporrted");
            return result;
        }

        public void CommitObject(DTOObject dtoObject)
        {
            var mrg = GetEntityManager(dtoObject);

            if (dtoObject.IsDeleted)
            {
                mrg.Delete(dtoObject.Key);
            }
            else
            {
                mrg.Save(dtoObject);
            }
        }
    }

    public class InvokeEventArgs : EventArgs
    {
        public InvokeEventArgs(Environment env, bool isServer, bool isOwner, bool register = true)
        {
            Env = env;

            Cancel = false;
            IsServer = isServer;
            Register = register;
            IsOwner = isOwner && !isServer;
        }

        public Environment Env { get; set; }

        public bool Cancel { get; set; }
        public bool IsServer { get; set; }
        public bool IsOwner { get; set; }
        public bool Register { get; set; }
    }
}
