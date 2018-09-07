﻿namespace ZenPlatform.QueryBuilder.Common.Operations
{
    public class VarbinaryTypeDefinitionNode : TypeDefinitionNode
    {
        public VarbinaryTypeDefinitionNode(int size) : base("varbinary")
        {
            Childs.Add(Tokens.Tokens.LeftBracketToken);

            if (size == 0)
                Childs.Add(new RawSqlNode("MAX"));
            else
                Childs.Add(new RawSqlNode(size.ToString()));

            Childs.Add(Tokens.Tokens.RightBracketToken);
        }
    }
}