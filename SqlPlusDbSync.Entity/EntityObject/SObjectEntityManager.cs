using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform.Configuration;

namespace SqlPlusDbSync.Platform.EntityObject
{
    public class SObjectEntityManager
    {
        private readonly AsnaDatabaseContext _context;
        private readonly SType _sobject;

        private Mapper _mapper;
        private SObjectQueryProcessor _processor;
        private Environment env;

        public SObjectEntityManager(AsnaDatabaseContext context, SType sobject)
        {
            _context = context;
            _sobject = sobject;
            _mapper = new Mapper();
            _processor = new SObjectQueryProcessor(_context);
            AfterLoad += OnAfterLoad;
            BeforeSave += OnBeforeSave;
            BeforeDelete += OnBeforeDelete;
            env = new Environment(_context, this);
        }


        public SType SType => _sobject;


        public DTOObject Create()
        {
            var asm = Assembly.Load("EntityAssembly");
            var type = asm.GetTypes().Single(x => x.Name == _sobject.Name);

            DTOObject dtoObject = Activator.CreateInstance(type) as DTOObject;

            return dtoObject;
        }

        public void Save(DTOObject obj)
        {
            var entity = obj as DTOObject;

            try
            {
                var args = new InvokeEventArgs(env, _context.IsServer, IsEntityOwner(entity));
                OnBeforeSaveNotify(entity, args);

                if (args.Cancel)
                    return;

                entity.Register = args.Register;

                var objGraph = LoadGraph(entity.Key) as DTOObject;
                MarkObject(entity, objGraph);

                var batch = _processor.GetSaveQuery(entity, _sobject);

                try
                {
                    _context.BeginTransaction(IsolationLevel.Snapshot);
                    _context.ExecNonQueryBatch(batch);
                    _context.CommitTransaction();
                }
                catch (Exception e)
                {
                    _context.RollbackTransaction();
                    throw e;
                }

                OnAfterSave(entity, args);
            }
            catch (Exception e)
            {
                OnOnError();
                throw e;
            }
        }
        public void Delete(object key)
        {
            var objGraph = LoadGraph(key) as DTOObject;

            if (objGraph.Key is null) return;

            var args = new InvokeEventArgs(env, _context.IsServer, IsEntityOwner(objGraph));

            OnBeforeDeleteNotify(objGraph, args);

            if (args.Cancel)
                return;

            objGraph.Register = args.Register;

            var batch = _processor.GetDeleteQuery(objGraph, _sobject);
            try
            {
                _context.BeginTransaction(IsolationLevel.Snapshot);
                _context.ExecNonQueryBatch(batch);
                _context.CommitTransaction();
            }
            catch (Exception e)
            {
                _context.RollbackTransaction();
                throw e;
            }

        }
        public DTOObject Load(object id)
        {
            try
            {
                var query = _processor.GetSingleSelectQuery(_sobject);
                query.Parameters[PlatformHelper.IdentityParameter].SetValue(id);


                var asm = Assembly.Load("EntityAssembly");
                var type = asm.GetTypes().Single(x => x.Name == _sobject.Name);

                DTOObject dtoObject = Activator.CreateInstance(type) as DTOObject;

                if (dtoObject is null) throw new Exception("This type not supported as Entity");

                dtoObject.Key = id;
                dtoObject.IsDeleted = false;

                using (var cmd = _context.CreateCommand(query.Compile()))
                {
                    foreach (var param in query.Parameters)
                    {
                        cmd.Parameters.Add(param.SqlParameter);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        dtoObject = _mapper.Map(dtoObject, _sobject, reader);
                    }

                    var args = new InvokeEventArgs(env, _context.IsServer, _context.LocatedPoints.Contains(GetObjectOwner(dtoObject)));

                    OnAfterLoadNotify(dtoObject, args);
                    dtoObject.DynamicProperties.Cancel = args.Cancel;
                    dtoObject.Register = args.Register;
                }

                return dtoObject;
            }
            catch (Exception e)
            {
                OnOnError();
                throw e;
            }
        }
        private DTOObject LoadGraph(object id)
        {
            try
            {

                var query = _processor.GetSingleSelectGraphQuery(_sobject);
                query.Parameters[PlatformHelper.IdentityParameter].SetValue(id);

                var asm = Assembly.Load("EntityAssembly");
                var type = asm.GetTypes().Single(x => x.Name == _sobject.Name);

                DTOObject result;
                using (var cmd = _context.CreateCommand(query.Compile()))
                {
                    foreach (var param in query.Parameters)
                    {
                        cmd.Parameters.Add(param.SqlParameter);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        result = _mapper.MapGraph(_sobject, reader, type);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                OnOnError();
                throw e;
            }
        }


        /// <summary>
        /// Mark  SQL query result to the object
        /// </summary>
        /// <param name="destenation">Destenation entity. For</param>
        /// <param name="source"></param>
        public void MarkObject(DTOObject destenation, DTOObject source)
        {
            if (source is null || source.Key is null)
            {
                destenation.DynamicProperties.Action = SaveAction.Insert;
            }
            else if (destenation is null || destenation.Key is null)
            {
                source.DynamicProperties.Action = SaveAction.Delete;
            }
            else
            {
                destenation.DynamicProperties.Action = SaveAction.Update;
            }

            var destType = (destenation ?? source).GetType();

            if (_sobject is TableType) return;

            foreach (var rel in _sobject.Relations)
            {
                var entityManager = new SObjectEntityManager(_context, rel.Type);

                var prop = destType.GetProperty(rel.Type.Name);
                var destValue = prop.GetValue(destenation ?? source, null) as IList;

                if (source is null || source.Key is null)
                {
                    foreach (DTOObject destItem in destValue)
                    {
                        entityManager.MarkObject(destItem, null);
                    }
                }
                else if (destenation is null || destenation.Key is null)
                {
                    foreach (DTOObject sourceItem in destValue)
                    {
                        entityManager.MarkObject(null, sourceItem);
                    }
                }
                else
                {
                    var sourceValue = prop.GetValue(source, null) as IList;

                    foreach (DTOObject destItem in destValue)
                    {
                        var sourceIndex = sourceValue.IndexOf(destItem);
                        if (sourceIndex >= 0)
                        {
                            entityManager.MarkObject(destItem, sourceValue[sourceIndex] as DTOObject);
                        }
                        else
                        {
                            entityManager.MarkObject(destItem, null);
                        }
                    }

                    foreach (DTOObject sourceItem in sourceValue)
                    {
                        if (!destValue.Contains(sourceItem))
                        {
                            var item = entityManager.Load(sourceItem.Key) as DTOObject;
                            entityManager.MarkObject(null, item);
                            destValue.Add(item);
                        }
                    }
                }
            }

        }

        private Guid GetObjectOwner(DTOObject dtoObject)
        {

            var sq = _processor.GetObjectOwnerQuery(_sobject);
            if (sq != null)
            {
                sq.Parameters[PlatformHelper.IdentityParameter].SetValue(dtoObject.Key);

                using (var cmd = _context.CreateCommand(sq.Compile()))
                {
                    foreach (var param in sq.Parameters)
                    {
                        cmd.Parameters.Add(param.SqlParameter);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.GetGuid(0);
                    }
                }
            }

            return Guid.Empty;
        }

        private bool IsEntityOwner(DTOObject dtoObject)
        {
            return _context.LocatedPoints.Contains(GetObjectOwner(dtoObject));
        }

        public event EventHandler<InvokeEventArgs> BeforeSave;
        public event EventHandler<InvokeEventArgs> AfterSave;
        public event EventHandler<InvokeEventArgs> AfterLoad;
        public event EventHandler<InvokeEventArgs> BeforeDelete;

        public event EventHandler<EventArgs> OnError;

        protected virtual void OnBeforeSaveNotify(object sender, InvokeEventArgs args)
        {
            BeforeSave?.Invoke(sender, args);
        }
        protected virtual void OnAfterLoadNotify(object sender, InvokeEventArgs args)
        {
            AfterLoad?.Invoke(sender, args);
        }
        protected virtual void OnOnError()
        {
            OnError?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnAfterSave(object sender, InvokeEventArgs args)
        {
            AfterSave?.Invoke(sender, args);
        }
        protected virtual void OnBeforeDeleteNotify(object sender, InvokeEventArgs args)
        {
            BeforeDelete?.Invoke(sender, args);
        }

        private void OnBeforeSave(object sender, InvokeEventArgs invokeEventArgs)
        {
            foreach (var ev in _sobject.Events.Where(x => x.Name == PlatformHelper.BeforeSave))
            {
                DynamicExecutor de = new DynamicExecutor();
                de.ExecuteCode(ev.Body, sender as DTOObject, invokeEventArgs);
            }
        }
        private void OnAfterLoad(object sender, InvokeEventArgs invokeEventArgs)
        {

            foreach (var ev in _sobject.Events.Where(x => x.Name == PlatformHelper.AfterLoad))
            {
                DynamicExecutor de = new DynamicExecutor();
                de.ExecuteCode(ev.Body, sender as DTOObject, invokeEventArgs);
            }
        }

        private void OnBeforeDelete(object sender, InvokeEventArgs invokeEventArgs)
        {
            foreach (var ev in _sobject.Events.Where(x => x.Name == PlatformHelper.BeforeDelete))
            {
                DynamicExecutor de = new DynamicExecutor();
                de.ExecuteCode(ev.Body, sender as DTOObject, invokeEventArgs);
            }
        }

    }

    public class Environment
    {
        public Environment(AsnaDatabaseContext context, SObjectEntityManager manager)
        {
            Context = context;
            Manager = manager;
        }

        public AsnaDatabaseContext Context { get; private set; }
        public SObjectEntityManager Manager { get; private set; }

    }
}