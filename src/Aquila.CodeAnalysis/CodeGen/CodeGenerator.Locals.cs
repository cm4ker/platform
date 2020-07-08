using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Diagnostics;
using Aquila.CodeAnalysis.Semantics;

namespace Aquila.CodeAnalysis.CodeGen
{
    partial class CodeGenerator
    {
        #region GetTemporaryLocal

        /// <summary>
        /// Returns a <see cref="LocalDefinition"/> of a temporary local variable of a specified <see cref="TypeSymbol"/>.
        /// </summary>
        /// <param name="type">The requested <see cref="TypeSymbol"/> of the local.</param>
        /// <param name="immediateReturn"><c>True</c> to immediately return the local builder to the pool of locals
        /// available for reuse (no need to call <see cref="ReturnTemporaryLocal(LocalDefinition)"/>).</param>
        /// <returns>The <see cref="LocalDefinition"/>.</returns>
        /// <remarks>
        /// If a <see cref="LocalDefinition"/> of the given <see cref="TypeSymbol"/> has already been declared and returned
        /// to the pool, this local is reused. Otherwise, a new local is declared. Use this method to obtain a
        /// short-lived temporary local. If <paramref name="immediateReturn"/> is <c>false</c>, return the local
        /// to the pool of locals available for reuse by calling <see cref="ReturnTemporaryLocal(LocalDefinition)"/>.
        /// </remarks>
        public LocalDefinition /*!*/ GetTemporaryLocal(TypeSymbol /*!*/ type, bool immediateReturn = false)
        {
            Debug.Assert(type.SpecialType != SpecialType.System_Void, "Variable cannot be of type 'void'!");

            var definition =
                _il.LocalSlotManager.AllocateSlot((Microsoft.Cci.ITypeReference) type, LocalSlotConstraints.None);

            if (immediateReturn)
            {
                _il.LocalSlotManager.FreeSlot(definition);
            }

            return definition;
        }

        /// <summary>
        /// Returns a <see cref="LocalDefinition"/> previously obtained from <see cref="GetTemporaryLocal(TypeSymbol,bool)"/> to the
        /// pool of locals available for reuse.
        /// </summary>
        /// <param name="definition">The <see cref="LocalDefinition"/> to return to the pool.</param>
        public void ReturnTemporaryLocal(LocalDefinition /*!*/ definition)
        {
            _il.LocalSlotManager.FreeSlot(definition);
        }

        #endregion


        /// <summary>
        /// Definition of a temporary local variable.
        /// The variable can be a local variable or a special temporary local variable living in array.
        /// </summary>
        public class TemporaryLocalDefinition : IPlace, IDisposable
        {
            CodeGenerator _cg;

            LocalDefinition _loc;

            string _tempName;
            TypeSymbol _type;

            public bool IsValid => _tempName != null || _loc != null;

            public TemporaryLocalDefinition(CodeGenerator cg, LocalDefinition loc)
            {
                _cg = cg;
                _loc = loc;
                _type = (TypeSymbol) loc.Type;
            }

            public TemporaryLocalDefinition(CodeGenerator cg, string tempname, TypeSymbol type)
            {
                _cg = cg;
                _tempName = tempname;
                _type = type;
            }

            public TypeSymbol Type => _type;

            public bool HasAddress => _loc != null;

            public void Dispose()
            {
                if (_loc != null)
                {
                    _cg.ReturnTemporaryLocal(_loc);

                    _loc = null;
                }
                else if (_tempName != null)
                {
                }
            }

            public TypeSymbol EmitLoad(ILBuilder il)
            {
                if (_loc != null)
                {
                    il.EmitLocalLoad(_loc);
                }
                else
                {
                }

                return Type;
            }

            /// <summary>
            /// Stores the value from top of the stack into this temporary local variable.
            /// </summary>
            public void EmitStore()
            {
                if (_loc != null)
                {
                    _cg.Builder.EmitLocalStore(_loc);
                }
                else
                {
                }
            }

            void IPlace.EmitStorePrepare(ILBuilder il)
            {
                throw new NotImplementedException();
            }

            void IPlace.EmitStore(ILBuilder il)
            {
                throw new NotImplementedException();
            }

            public void EmitLoadAddress(ILBuilder il)
            {
                if (_loc != null)
                {
                    il.EmitLocalAddress(_loc);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }


        /// <summary>
        /// If possible, gets <see cref="IPlace"/> representing given expression (in case of a field or variable).
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>Place or <c>null</c>.</returns>
        internal IPlace PlaceOrNull(BoundExpression expr)
        {
            throw new NotImplementedException();
            //return expr is BoundReferenceExpression bref ? bref.Place() : null;
        }
    }
}