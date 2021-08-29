using System.Data.Common;
using System.Threading;

namespace Aquila.Core
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