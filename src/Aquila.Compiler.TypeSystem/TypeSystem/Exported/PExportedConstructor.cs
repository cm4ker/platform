using System;

namespace Aquila.Compiler.Aqua.TypeSystem.Exported
{
    public class PExportedConstructor : PConstructor
    {
        public PExportedConstructor(Guid parentId, TypeManager tm) : base(parentId, tm)
        {
        }

        public PExportedConstructor(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
        }
    }
}