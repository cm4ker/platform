using System;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Aqua.TypeSystem.Exported
{
    public class PExportedConstructor : PConstructor
    {
        private readonly IConstructor _c;

        public PExportedConstructor(IConstructor c, TypeManager tm) : base(Guid.Empty, Guid.Empty, tm)
        {
            _c = c;
        }
    }
}