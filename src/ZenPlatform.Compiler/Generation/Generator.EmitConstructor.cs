using System;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitConstructor(Constructor constructor, RoslynTypeBuilder type,
            RoslynConstructorBuilder stage1Constructor)
        {
            constructor.Builder = stage1Constructor.Body;

            EmitConstructor(constructor, type);
        }

        private void EmitConstructor(Constructor constructor, RoslynTypeBuilder type)
        {
            if (constructor == null)
                throw new ArgumentNullException();

            var emitter = constructor.Builder;

            ILocal resultVar = null;

            // var returnLabel = emitter.DefineLabel();
            //
            // //Используем конструктор по умолчанию
            // //emitter.LdArg_0().EmitCall(_bindings.Object.Constructors[0]);
            // emitter.LdArg_0().EmitCall(type.BaseType.Constructors[0]);
            //
            // EmitBody(emitter, constructor.Block, returnLabel, ref resultVar);
            // emitter.MarkLabel(returnLabel);
            //
            // emitter.Ret();
        }
    }
}