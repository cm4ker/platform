using System;

namespace QueryCompiler
{
    public class FieldNotFoundException : Exception
    {
        public FieldNotFoundException(string fieldName) : base($"The field {fieldName} not found in this query")
        {

        }
    }
}