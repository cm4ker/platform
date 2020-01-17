using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    public abstract class XCLinkTypeBase : XCTypeBase, IXCLinkType
    {
        public bool IsLink => true;

        public virtual bool IsAbstract => true;

        public virtual bool IsSealed => throw new NotImplementedException();

        public IXCType BaseType { get; }

        public virtual bool HasProperties => throw new NotImplementedException();

        public virtual bool HasModules => throw new NotImplementedException();

        public virtual bool HasCommands => throw new NotImplementedException();

        public virtual bool HasDatabaseUsed => throw new NotImplementedException();

        public void Initialize()
        {
            //throw new NotImplementedException();
        }

        public void LoadDependencies()
        {
            
        }


        public virtual IEnumerable<IXCProperty> GetProperties()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IXCTable> GetTables()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IXCProgramModule> GetProgramModules()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IXCCommand> GetCommands()
        {
            throw new NotImplementedException();
        }

        public IXCComponent Parent { get; set; }

        public IXCObjectType ParentType { get; protected set; }

    }
}