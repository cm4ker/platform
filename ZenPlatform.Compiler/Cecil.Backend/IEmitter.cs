using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ZenPlatform.Compiler.Cecil.Backend
{
    public interface IEmitter
    {
        Emitter Add();
        Emitter Sub();
        Emitter Mul();
        Emitter Div();
        Emitter Rem();

        /// <summary>
        /// Сравнивает два значения со стека
        /// </summary>
        /// <returns></returns>
        Emitter Ceq();

        Emitter Cgt();
        Emitter Clt();
        Emitter And();
        Emitter Or();
        Emitter Neg();
        Emitter Not();
        Emitter LdcI4(int i);

        /// <summary>
        /// Загружает индекс элемента массива на стэк
        /// </summary>
        /// <returns></returns>
        Emitter LdElemI4();

        Emitter LdElemA();

        /// <summary>
        /// Получает из стэка индекс массива
        /// </summary>
        /// <returns></returns>
        Emitter StElemI4();

        Emitter LdStr(string value);
        Emitter LdcR8(double d);
        Emitter LdcR4(float f);

        /// <summary>
        /// Положить на стэк параметр с определенным индексом
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Emitter LdArg(int i);

        Emitter LdArgA(int i);
        Emitter StArg(ParameterDefinition pd);
        Emitter StIndI4();
        Emitter LdIndI4();
        Emitter StLoc(VariableDefinition vd);
        Emitter LdLoc(VariableDefinition vd);
        Emitter LdLocA(VariableDefinition vd);
        Emitter StsFld(FieldDefinition fd);
        Emitter LdsFld(FieldDefinition fd);
        Emitter LdsFldA(FieldDefinition fd);
        Emitter LdFld(FieldReference fr);
        Emitter NewArr(TypeReference tr);
        Emitter Br(Label label);
        Emitter BrFalse(Label label);
        Emitter BrTrue(Label label);
        Emitter Pop();
        Emitter Call(MethodDefinition md);
        Emitter NotEqual();
        Emitter GreaterOrEqual();
        Emitter LessOrEqual();
        void Append(Instruction instruction);
        void Append(Label label);
        void Variable(VariableDefinition vd);
        MethodBody MethodBody { get; }
        Emitter ConvI4();
        Emitter ConvR4();
        Emitter ConvR8();
        Emitter ConvU2();
    }
}