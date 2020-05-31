using System;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Aqua.TypeSystem.Exported
{
    public class PExportedMethod : PMethod
    {
        //TODO: Set type identifier as Guid instead Object
        internal PExportedMethod(IMethod method, TypeManager tm) : base(Guid.NewGuid(), Guid.Empty, tm)
        {
        }
    }
}