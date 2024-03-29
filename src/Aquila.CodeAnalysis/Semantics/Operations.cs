namespace Aquila.Syntax.Ast
{
    public enum Operations
    {
        Unknown,

        // unary ops:
        Plus,
        Minus,
        LogicNegation,
        BitNegation,
        AtSign,
        Print,
        Clone,

        // casts:
        BoolCast,
        Int8Cast,
        Int16Cast,
        Int32Cast,
        Int64Cast,
        UInt8Cast,
        UInt16Cast,
        UInt32Cast,
        UInt64Cast,
        DoubleCast,
        FloatCast,
        DecimalCast,
        StringCast,
        BinaryCast,
        UnicodeCast,
        ObjectCast,
        ArrayCast,
        UnsetCast,

        // binary ops:
        Xor,
        Or,
        And,
        BitOr,
        BitXor,
        BitAnd,
        Equal,
        NotEqual,
        Identical,
        NotIdentical,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
        ShiftLeft,
        ShiftRight,
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        Pow,
        Concat,
        Spaceship,
        Coalesce,

        // n-ary ops:
        ConcatN,
        List,
        Conditional,

        // assignments:
        AssignRef,
        AssignValue,
        AssignAdd,
        AssignSub,
        AssignMul,
        AssignPow,
        AssignDiv,
        AssignMod,
        AssignAnd,
        AssignOr,
        AssignXor,
        AssignShiftLeft,
        AssignShiftRight,
        AssignAppend,
        AssignPrepend,
        AssignCoalesce,

        // constants, variables, fields, items:
        MemberAccess,
        Indexer,

        // literals:
        NullLiteral,
        BoolLiteral,
        IntLiteral,
        CharLiteral,
        LongIntLiteral,
        DoubleLiteral,
        StringLiteral,
        BinaryStringLiteral,
        QueryLiteral,

        // calls:
        Call,

        // instances:
        New,
        Array,
        InstanceOf,
        TypeOf,

        // built-in functions:
        Inclusion,
        Isset,
        Empty,

        // others:
        Exit,
        ShellCommand,
        IncDec,
        Yield,
        Throw,
        Parenthesis,
        Match,

        // lambda function:
        Closure,
        ArrowFunc,
    }
}