using System.Data.Common;

namespace ZenPlatform.Core.Helpers
{
    public static class DatabaseWorkHelper
    {
        public static void AddParameterWithValue(this DbCommand cmd, string parameterName,
            object parameterValue)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;

            cmd.Parameters.Add(parameter);
        }
    }
}