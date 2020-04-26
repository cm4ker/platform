using System;

namespace ZenPlatform.QueryBuilder
{
    public class FieldNotFoundException : Exception
    {
        public FieldNotFoundException(string fieldName) : base($"The field {fieldName} not found in this query")
        {

        }
    }
}