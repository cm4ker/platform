using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Describes a type.
    /// </summary>
    public class Type
    {
        /// <summary>
        /// Name of type, generally name of structure if VariableType is Structure.
        /// </summary>
        public string Name;

        /// <summary>
        /// Variable type.
        /// </summary>
        public VariableType VariableType = VariableType.Primitive;

        /// <summary>
        /// Primitive type if VariableType is Primitive.
        /// </summary>
        public PrimitiveType PrimitiveType = PrimitiveType.Void;

        public bool IsRef = false;

        /// <summary>
        /// Create a primitive type.
        /// </summary>
        /// <param name="primitiveType"></param>
        public Type(PrimitiveType primitiveType)
        {
            VariableType = VariableType.Primitive;
            PrimitiveType = primitiveType;
        }

        /// <summary>
        /// Create a structure type.
        /// </summary>
        /// <param name="name"></param>
        public Type(string name)
        {
            VariableType = VariableType.Structure;
            Name = name;
        }

        /// <summary>
        /// Creates a .NET system type from type.
        /// </summary>
        /// <returns></returns>
        public System.Type ToSystemType()
        {
            if (!IsRef)
            {
                if (VariableType == VariableType.Primitive)
                {
                    if (PrimitiveType == PrimitiveType.Integer)
                        return typeof(int);
                    else if (PrimitiveType == PrimitiveType.Boolean)
                        return typeof(bool);
                    else if (PrimitiveType == PrimitiveType.Character)
                        return typeof(char);
                    else if (PrimitiveType == PrimitiveType.Double)
                        return typeof(double);
                    else if (PrimitiveType == PrimitiveType.String)
                        return typeof(string);
                    else if (PrimitiveType == PrimitiveType.Void)
                        return typeof(void);
                }
                else if (VariableType == VariableType.PrimitiveArray)
                {
                    if (PrimitiveType == PrimitiveType.Integer)
                        return typeof(int[]);
                    else if (PrimitiveType == PrimitiveType.Boolean)
                        return typeof(bool[]);
                    else if (PrimitiveType == PrimitiveType.Character)
                        return typeof(char[]);
                    else if (PrimitiveType == PrimitiveType.Real)
                        return typeof(float[]);
                }
            }
            else
            {
                if (VariableType == VariableType.Primitive)
                {
                    if (PrimitiveType == PrimitiveType.Integer)
                        return System.Type.GetType("System.Int32&");
                    else
                        return typeof(void);
                }
                else if (VariableType == VariableType.PrimitiveArray)
                {
                    if (PrimitiveType == PrimitiveType.Integer)
                        return System.Type.GetType("System.Int32[]&");
                    else
                        return typeof(void);
                }
            }

            return null;
        }

        private Type()
        {
        } // Hidden default constructor.

        /// <summary>
        /// Create a primitive array type from a non array type.
        /// </summary>
        public static Type CreateArrayFromType(Type type)
        {
            if (type.VariableType == VariableType.Primitive)
                type.VariableType = VariableType.PrimitiveArray;
            else if (type.VariableType == VariableType.Structure)
                type.VariableType = VariableType.StructureArray;
            return type;
        }

        /// <summary>
        /// Create a primitive array type.
        /// </summary>
        public static Type CreateArrayType(PrimitiveType primitiveType)
        {
            Type newType = new Type();
            newType.VariableType = VariableType.PrimitiveArray;
            newType.PrimitiveType = primitiveType;
            return newType;
        }

        /// <summary>
        /// Create a structure array type.
        /// </summary>
        public static Type CreateArrayType(string name)
        {
            Type newType = new Type();
            newType.VariableType = VariableType.StructureArray;
            newType.Name = name;
            return newType;
        }


        #region Primitive types

        public static Type String = new Type(PrimitiveType.String);
        public static Type Character = new Type(PrimitiveType.Character);
        public static Type Int = new Type(PrimitiveType.Integer);
        public static Type Double = new Type(PrimitiveType.Double);
        public static Type Void = new Type(PrimitiveType.Void);
        public static Type Bool = new Type(PrimitiveType.Boolean);
        public static Type Real = new Type(PrimitiveType.Real);

        #endregion
    }
}