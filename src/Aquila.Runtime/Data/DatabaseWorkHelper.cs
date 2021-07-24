using System.Data.Common;
using System.Linq;
using Aquila.Core.Querying.Model;

namespace Aquila.Data
{
    public static class DatabaseWorkHelper
    {
        public static void AddOrSetParameterWithValue(this DbCommand cmd, string parameterName,
            object parameterValue)
        {
            var exists = cmd.FindDbParameter(parameterName);

            var parameter = exists ?? cmd.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;

            if (exists is null)
                cmd.Parameters.Add(parameter);
        }

        public static DbParameter FindDbParameter(this DbCommand cmd, string paramName)
        {
            foreach (DbParameter param in cmd.Parameters)
            {
                if (param.ParameterName == paramName) return param;
            }

            return null;
        }
    }
}