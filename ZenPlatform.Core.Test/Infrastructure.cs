using System;
using ZenPlatform.ClientRuntime;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Core.Contracts;

namespace ZenPlatform.Core.Test
{
    public class EntityLink : ILink
    {
        public EntityLink(ViewBag bag)
        {
            if (bag.Has("Id"))
                Id = bag.Get<Guid>("Id");
    
            if (bag.Has("Type"))
                TypeId = bag.Get<int>("Type");
        }
    
        public Guid Id { get; }
    
        public int TypeId { get; }
    
        public virtual string Name => "Entity";
    
        public string Presentation => ToString();
    
        public override string ToString()
        {
            return $"{Name} = ({TypeId}:{{{Id}}})";
        }
    }
    
    public class StoreLink : EntityLink
    {
        public StoreLink(ViewBag bag) : base(bag)
        {
        }
    }
    
    public class InvoiceLink : EntityLink
    {
        private StoreLink _store;
    
        public InvoiceLink(ViewBag bag) : base(bag)
        {
        }
    
        public StoreLink Store => _store ??=
            new StoreLink(GlobalScope.Client.Invoke<ViewBag>(new Route("Test_GetProperty"), TypeId, nameof(Store), Id));
    }
}