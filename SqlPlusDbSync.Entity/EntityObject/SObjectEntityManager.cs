//using System;
//using System.Collections;
//using System.Data;
//using System.Linq;
//using System.Reflection;
//using SqlPlusDbSync.Data.Database;
//using SqlPlusDbSync.Entity.EntityObject;
//using SqlPlusDbSync.Platform.Configuration;

//namespace SqlPlusDbSync.Platform.EntityObject
//{
//    public class SObjectEntityManager
//    {
//        private readonly AsnaDatabaseContext _context;
//        private readonly SType _sobject;

//        private Mapper _mapper;
//        private SObjectQueryProcessor _processor;
//        private Environment env;

//        public SObjectEntityManager(AsnaDatabaseContext context, SType sobject)
//        {
//            _context = context;
//            _sobject = sobject;
//            _mapper = new Mapper();
//            _processor = new SObjectQueryProcessor(_context);
//            AfterLoad += OnAfterLoad;
//            BeforeSave += OnBeforeSave;
//            BeforeDelete += OnBeforeDelete;
//            env = new Environment(_context, this);
//        }


//        public SType SType => _sobject;


//        public DTO Create()
//        {
//            var asm = Assembly.Load("EntityAssembly");
//            var type = asm.GetTypes().Single(x => x.Name == _sobject.Name);

//            DTO dto = Activator.CreateInstance(type) as DTO;

//            return dto;
//        }

//        public void Save(DTO obj)
//        {
//            var entity = obj as DTO;

//            try
//            {
//                var args = new InvokeEventArgs(env, _context.IsServer, IsEntityOwner(entity));
//                OnBeforeSaveNotify(entity, args);

//                if (args.Cancel)
//                    return;

//                entity.Register = args.Register;

//                var objGraph = LoadGraph(entity.Key) as DTO;
//                MarkObject(entity, objGraph);

//                var batch = _processor.GetSaveQuery(entity, _sobject);

//                try
//                {
//                    _context.BeginTransaction(IsolationLevel.Snapshot);
//                    _context.ExecNonQueryBatch(batch);
//                    _context.CommitTransaction();
//                }
//                catch (Exception e)
//                {
//                    _context.RollbackTransaction();
//                    throw e;
//                }

//                OnAfterSave(entity, args);
//            }
//            catch (Exception e)
//            {
//                OnOnError();
//                throw e;
//            }
//        }
//        public void Delete(object key)
//        {
//            var objGraph = LoadGraph(key) as DTO;

//            if (objGraph.Key is null) return;

//            var args = new InvokeEventArgs(env, _context.IsServer, IsEntityOwner(objGraph));

//            OnBeforeDeleteNotify(objGraph, args);

//            if (args.Cancel)
//                return;

//            objGraph.Register = args.Register;

//            var batch = _processor.GetDeleteQuery(objGraph, _sobject);
//            try
//            {
//                _context.BeginTransaction(IsolationLevel.Snapshot);
//                _context.ExecNonQueryBatch(batch);
//                _context.CommitTransaction();
//            }
//            catch (Exception e)
//            {
//                _context.RollbackTransaction();
//                throw e;
//            }

//        }
//        public DTO Load(object id)
//        {
//            try
//            {
//                var query = _processor.GetSingleSelectQuery(_sobject);
//                query.Parameters[PlatformHelper.IdentityParameter].SetValue(id);


//                var asm = Assembly.Load("EntityAssembly");
//                var type = asm.GetTypes().Single(x => x.Name == _sobject.Name);

//                DTO dto = Activator.CreateInstance(type) as DTO;

//                if (dto is null) throw new Exception("This type not supported as Entity");

//                dto.Key = id;
//                dto.IsDeleted = false;

//                using (var cmd = _context.CreateCommand(query.Compile()))
//                {
//                    foreach (var param in query.Parameters)
//                    {
//                        cmd.Parameters.Add(param.SqlParameter);
//                    }
//                    using (var reader = cmd.ExecuteReader())
//                    {
//                        dto = _mapper.Map(dto, _sobject, reader);
//                    }

//                    var args = new InvokeEventArgs(env, _context.IsServer, _context.LocatedPoints.Contains(GetObjectOwner(dto)));

//                    OnAfterLoadNotify(dto, args);
//                    dto.DynamicProperties.Cancel = args.Cancel;
//                    dto.Register = args.Register;
//                }

//                return dto;
//            }
//            catch (Exception e)
//            {
//                OnOnError();
//                throw e;
//            }
//        }
//        private DTO LoadGraph(object id)
//        {
//            try
//            {

//                var query = _processor.GetSingleSelectGraphQuery(_sobject);
//                query.Parameters[PlatformHelper.IdentityParameter].SetValue(id);

//                var asm = Assembly.Load("EntityAssembly");
//                var type = asm.GetTypes().Single(x => x.Name == _sobject.Name);

//                DTO result;
//                using (var cmd = _context.CreateCommand(query.Compile()))
//                {
//                    foreach (var param in query.Parameters)
//                    {
//                        cmd.Parameters.Add(param.SqlParameter);
//                    }
//                    using (var reader = cmd.ExecuteReader())
//                    {
//                        result = _mapper.MapGraph(_sobject, reader, type);
//                    }
//                }
//                return result;
//            }
//            catch (Exception e)
//            {
//                OnOnError();
//                throw e;
//            }
//        }


//        /// <summary>
//        /// Mark  SQL query result to the object
//        /// </summary>
//        /// <param name="destenation">Destenation entity. For</param>
//        /// <param name="source"></param>
//        public void MarkObject(DTO destenation, DTO source)
//        {
//            if (source is null || source.Key is null)
//            {
//                destenation.DynamicProperties.Action = SaveAction.Insert;
//            }
//            else if (destenation is null || destenation.Key is null)
//            {
//                source.DynamicProperties.Action = SaveAction.Delete;
//            }
//            else
//            {
//                destenation.DynamicProperties.Action = SaveAction.Update;
//            }

//            var destType = (destenation ?? source).GetType();

//            if (_sobject is TableType) return;

//            foreach (var rel in _sobject.Relations)
//            {
//                var entityManager = new SObjectEntityManager(_context, rel.Type);

//                var prop = destType.GetProperty(rel.Type.Name);
//                var destValue = prop.GetValue(destenation ?? source, null) as IList;

//                if (source is null || source.Key is null)
//                {
//                    foreach (DTO destItem in destValue)
//                    {
//                        entityManager.MarkObject(destItem, null);
//                    }
//                }
//                else if (destenation is null || destenation.Key is null)
//                {
//                    foreach (DTO sourceItem in destValue)
//                    {
//                        entityManager.MarkObject(null, sourceItem);
//                    }
//                }
//                else
//                {
//                    var sourceValue = prop.GetValue(source, null) as IList;

//                    foreach (DTO destItem in destValue)
//                    {
//                        var sourceIndex = sourceValue.IndexOf(destItem);
//                        if (sourceIndex >= 0)
//                        {
//                            entityManager.MarkObject(destItem, sourceValue[sourceIndex] as DTO);
//                        }
//                        else
//                        {
//                            entityManager.MarkObject(destItem, null);
//                        }
//                    }

//                    foreach (DTO sourceItem in sourceValue)
//                    {
//                        if (!destValue.Contains(sourceItem))
//                        {
//                            var item = entityManager.Load(sourceItem.Key) as DTO;
//                            entityManager.MarkObject(null, item);
//                            destValue.Add(item);
//                        }
//                    }
//                }
//            }

//        }

//        private Guid GetObjectOwner(DTO dto)
//        {

//            var sq = _processor.GetObjectOwnerQuery(_sobject);
//            if (sq != null)
//            {
//                sq.Parameters[PlatformHelper.IdentityParameter].SetValue(dto.Key);

//                using (var cmd = _context.CreateCommand(sq.Compile()))
//                {
//                    foreach (var param in sq.Parameters)
//                    {
//                        cmd.Parameters.Add(param.SqlParameter);
//                    }
//                    using (var reader = cmd.ExecuteReader())
//                    {
//                        return reader.GetGuid(0);
//                    }
//                }
//            }

//            return Guid.Empty;
//        }

//        private bool IsEntityOwner(DTO dto)
//        {
//            return _context.LocatedPoints.Contains(GetObjectOwner(dto));
//        }

//        public event EventHandler<InvokeEventArgs> BeforeSave;
//        public event EventHandler<InvokeEventArgs> AfterSave;
//        public event EventHandler<InvokeEventArgs> AfterLoad;
//        public event EventHandler<InvokeEventArgs> BeforeDelete;

//        public event EventHandler<EventArgs> OnError;

//        protected virtual void OnBeforeSaveNotify(object sender, InvokeEventArgs args)
//        {
//            BeforeSave?.Invoke(sender, args);
//        }
//        protected virtual void OnAfterLoadNotify(object sender, InvokeEventArgs args)
//        {
//            AfterLoad?.Invoke(sender, args);
//        }
//        protected virtual void OnOnError()
//        {
//            OnError?.Invoke(this, EventArgs.Empty);
//        }
//        protected virtual void OnAfterSave(object sender, InvokeEventArgs args)
//        {
//            AfterSave?.Invoke(sender, args);
//        }
//        protected virtual void OnBeforeDeleteNotify(object sender, InvokeEventArgs args)
//        {
//            BeforeDelete?.Invoke(sender, args);
//        }

//        private void OnBeforeSave(object sender, InvokeEventArgs invokeEventArgs)
//        {
//            foreach (var ev in _sobject.Events.Where(x => x.Name == PlatformHelper.BeforeSave))
//            {
//                DynamicExecutor de = new DynamicExecutor();
//                de.ExecuteCode(ev.Body, sender as DTO, invokeEventArgs);
//            }
//        }
//        private void OnAfterLoad(object sender, InvokeEventArgs invokeEventArgs)
//        {

//            foreach (var ev in _sobject.Events.Where(x => x.Name == PlatformHelper.AfterLoad))
//            {
//                DynamicExecutor de = new DynamicExecutor();
//                de.ExecuteCode(ev.Body, sender as DTO, invokeEventArgs);
//            }
//        }

//        private void OnBeforeDelete(object sender, InvokeEventArgs invokeEventArgs)
//        {
//            foreach (var ev in _sobject.Events.Where(x => x.Name == PlatformHelper.BeforeDelete))
//            {
//                DynamicExecutor de = new DynamicExecutor();
//                de.ExecuteCode(ev.Body, sender as DTO, invokeEventArgs);
//            }
//        }

//    }

//    public class Environment
//    {
//        public Environment(AsnaDatabaseContext context, SObjectEntityManager manager)
//        {
//            Context = context;
//            Manager = manager;
//        }

//        public AsnaDatabaseContext Context { get; private set; }
//        public SObjectEntityManager Manager { get; private set; }

//    }
//}