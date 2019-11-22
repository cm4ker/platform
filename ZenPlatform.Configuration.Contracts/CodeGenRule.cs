using System;

namespace ZenPlatform.Configuration.Contracts
{
    public class CodeGenRule
    {
        private readonly string _expression;

        public CodeGenRule(CodeGenRuleType type, string expression)
        {
            Type = type;
            _expression = expression;
        }

        public CodeGenRuleType Type { get; }

        public string GetExpression()
        {
            if (_expression is null) throw new NotImplementedException();
            return _expression;
        }
    }
}