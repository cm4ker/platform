using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.Cecil.Backend
{
    public class Emitter
    {
        private readonly ILProcessor _ce;

        public Emitter(ILProcessor ce)
        {
            _ce = ce;
        }

        #region Primitive instructions

        public Emitter Add()
        {
            _ce.Emit(OpCodes.Add);
            return this;
        }

        public Emitter Sub()
        {
            _ce.Emit(OpCodes.Sub);
            return this;
        }

        public Emitter Mul()
        {
            _ce.Emit(OpCodes.Mul);
            return this;
        }

        public Emitter Div()
        {
            _ce.Emit(OpCodes.Div);
            return this;
        }

        public Emitter Rem()
        {
            _ce.Emit(OpCodes.Rem);
            return this;
        }


        /// <summary>
        /// Сравнивает два значения со стека
        /// </summary>
        /// <returns></returns>
        public Emitter Ceq()
        {
            _ce.Emit(OpCodes.Ceq);
            return this;
        }

        public Emitter Cgt()
        {
            _ce.Emit(OpCodes.Cgt);
            return this;
        }

        public Emitter Clt()
        {
            _ce.Emit(OpCodes.Clt);
            return this;
        }

        public Emitter And()
        {
            _ce.Emit(OpCodes.And);
            return this;
        }

        public Emitter Or()
        {
            _ce.Emit(OpCodes.Or);
            return this;
        }

        public Emitter Neg()
        {
            _ce.Emit(OpCodes.Neg);

            return this;
        }

        public Emitter Not()
        {
            _ce.Emit(OpCodes.Not);

            return this;
        }

        public Emitter LdcI4(int i)
        {
            switch (i)
            {
                case 0:
                    _ce.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    _ce.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    _ce.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    _ce.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    _ce.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    _ce.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    _ce.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    _ce.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    _ce.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    _ce.Emit(OpCodes.Ldc_I4, i);
                    break;
            }


            return this;
        }

        /// <summary>
        /// Загружает индекс элемента массива на стэк
        /// </summary>
        /// <returns></returns>
        public Emitter LdElemI4()
        {
            _ce.Emit(OpCodes.Ldelem_I4);
            return this;
        }

        public Emitter LdElemA()
        {
            _ce.Emit(OpCodes.Ldelema);
            return this;
        }


        /// <summary>
        /// Получает из стэка индекс массива
        /// </summary>
        /// <returns></returns>
        public Emitter StElemI4()
        {
            _ce.Emit(OpCodes.Stelem_I4);
            return this;
        }

        public Emitter LdStr(string value)
        {
            _ce.Emit(OpCodes.Ldstr, value);

            return this;
        }

        public Emitter LdcR8(double d)
        {
            _ce.Emit(OpCodes.Ldc_R8, d);

            return this;
        }

        public Emitter LdcR4(float f)
        {
            _ce.Emit(OpCodes.Ldc_R4, f);

            return this;
        }

        /// <summary>
        /// Положить на стэк параметр с определенным индексом
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Emitter LdArg(int i)
        {
            if (i < 4)
                switch (i)
                {
                    case 0:
                        _ce.Emit(OpCodes.Ldarg_0);
                        break;
                    case 1:
                        _ce.Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        _ce.Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        _ce.Emit(OpCodes.Ldarg_3);
                        break;
                }
            else
                _ce.Emit(OpCodes.Ldarg_S, i);

            return this;
        }

        public Emitter LdArgA(int i)
        {
            _ce.Emit(OpCodes.Ldarga_S, i);
            return this;
        }


        public Emitter StArg(ParameterDefinition pd)
        {
            _ce.Emit(OpCodes.Starg, pd);
            return this;
        }

        public Emitter StIndI4()
        {
            _ce.Emit(OpCodes.Stind_I4);

            return this;
        }


        public Emitter LdIndI4()
        {
            _ce.Emit(OpCodes.Ldind_I4);

            return this;
        }

        public Emitter StLoc(VariableDefinition vd)
        {
            if (vd.Index > 255)

                _ce.Emit(OpCodes.Stloc, vd);
            else
                _ce.Emit(OpCodes.Stloc_S, vd);

            return this;
        }

        public Emitter LdLoc(VariableDefinition vd)
        {
            if (vd.Index > 255)

                _ce.Emit(OpCodes.Ldloc, vd);
            else
                _ce.Emit(OpCodes.Ldloc_S, vd);

            return this;
        }

        public Emitter LdLocA(VariableDefinition vd)
        {
            _ce.Emit(OpCodes.Ldloca, vd);
            return this;
        }

        public Emitter StsFld(FieldDefinition fd)
        {
            _ce.Emit(OpCodes.Stsfld, fd);

            return this;
        }

        public Emitter LdsFld(FieldDefinition fd)
        {
            _ce.Emit(OpCodes.Ldsfld, fd);

            return this;
        }

        public Emitter LdsFldA(FieldDefinition fd)
        {
            _ce.Emit(OpCodes.Ldsflda, fd);

            return this;
        }

        public Emitter LdFld(FieldReference fr)
        {
            _ce.Emit(OpCodes.Ldfld, fr);

            return this;
        }

        public Emitter NewArr(TypeReference tr)
        {
            _ce.Emit(OpCodes.Newarr, tr);

            return this;
        }

        public Emitter Br(Label label)
        {
            _ce.Emit(OpCodes.Br, label.Instruction);

            return this;
        }

        public Emitter BrFalse(Label label)
        {
            _ce.Emit(OpCodes.Brfalse, label.Instruction);

            return this;
        }

        public Emitter BrTrue(Label label)
        {
            _ce.Emit(OpCodes.Brtrue, label.Instruction);

            return this;
        }

        public Emitter Pop()
        {
            _ce.Emit(OpCodes.Pop);

            return this;
        }

        public Emitter Call(MethodReference md)
        {
            _ce.Emit(OpCodes.Call, md);

            return this;
        }

        #endregion

        #region Complex instructions

        public Emitter NotEqual()
        {
            Ceq();
            LdcI4(0);
            Ceq();
            return this;
        }

        public Emitter GreaterOrEqual()
        {
            Clt();
            LdcI4(0);
            Ceq();

            return this;
        }

        public Emitter LessOrEqual()
        {
            Cgt();
            LdcI4(0);
            Ceq();

            return this;
        }

        #endregion

        public void Append(Instruction instruction)
        {
            _ce.Append(instruction);
        }

        public void Append(Label label)
        {
            _ce.Append(label.Instruction);
        }


        public void Variable(VariableDefinition vd)
        {
            _ce.Body.Variables.Add(vd);
        }


        public MethodBody MethodBody => _ce.Body;

        #region Convert

        public Emitter ConvI4()
        {
            _ce.Emit(OpCodes.Conv_I4);

            return this;
        }

        public Emitter ConvR4()
        {
            _ce.Emit(OpCodes.Conv_R4);

            return this;
        }

        public Emitter ConvR8()
        {
            _ce.Emit(OpCodes.Conv_R8);

            return this;
        }

        public Emitter ConvU2()
        {
            _ce.Emit(OpCodes.Conv_U2);

            return this;
        }

        #endregion
    }
}