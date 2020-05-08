using System;

namespace Aquila.QueryBuilder
{
    /// <summary>
    /// This class is obsolete
    /// </summary>
    [Obsolete]
    public class DBSumCompileTransformation : DBCompileTransformation
    {
        public override string Apply(string token)
        {
            return token;
        }
    }
}