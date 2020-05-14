using System.Collections;
using System.Collections.Generic;

namespace Aquila.EntityComponent.Compilation
{
    public abstract class EntityTable<TDtoRowClass, TObjectClass> : IEnumerable<TObjectClass>
    {
        protected List<TDtoRowClass> DtoRef { get; }

        protected List<TObjectClass> ObjRef { get; }

        public EntityTable(List<TDtoRowClass> dtoRef)
        {
            DtoRef = dtoRef;
        }

        public abstract TObjectClass Add();

        // public abstract void Remove(TObjectClass item);

        public IEnumerator<TObjectClass> GetEnumerator()
        {
            return ObjRef.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}