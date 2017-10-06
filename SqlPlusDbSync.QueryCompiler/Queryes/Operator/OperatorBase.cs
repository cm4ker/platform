using System;

namespace SqlPlusDbSync.QueryCompiler.Queryes.Operator
{
    public abstract class OperatorBase : IOperator
    {
        public virtual string Compile()
        {
            throw new NotImplementedException();
        }

        public string Compile(CompileOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

