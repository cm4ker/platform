using System.Data.Common;
using System.Threading;

namespace Aquila.Core
{
    public class ContextHelper
    {
        private static AsyncLocal<PlatformContext> Context = new AsyncLocal<PlatformContext>();

        public static PlatformContext GetContext() => Context.Value;

        public static void SetContext(PlatformContext value) => Context.Value = value;
    }

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