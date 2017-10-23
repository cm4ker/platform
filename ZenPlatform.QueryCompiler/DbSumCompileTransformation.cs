using System;

namespace ZenPlatform.QueryCompiler
{
    /// <summary>
    /// This class is obsolete
    /// </summary>
    [Obsolete]
    public class DbSumCompileTransformation : DBCompileTransformation
    {
        public override string Apply(string token)
        {
            return token;
        }
    }
}