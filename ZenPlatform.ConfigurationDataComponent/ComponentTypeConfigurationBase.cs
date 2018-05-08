using System;

namespace ZenPlatform.DataComponent
{
    public class ComponentTypeConfigurationBase<TObjectType> : ComponentTypeConfigurationBase
    {
        public ComponentTypeConfigurationBase() : base(typeof(TObjectType))
        {
        }
    }

    public class ComponentTypeConfigurationBase<TObjectType, TComplexType> : ComponentTypeConfigurationBase
    {
        public ComponentTypeConfigurationBase() : base(typeof(TObjectType), typeof(TComplexType))
        {
        }
    }

    public abstract class ComponentTypeConfigurationBase
    {
        protected ComponentTypeConfigurationBase(Type objectType)
        {
            ObjectType = objectType;
        }

        protected ComponentTypeConfigurationBase(Type objectType, Type complexObjectType)
        {
            ObjectType = objectType;
            ComplexObjectType = complexObjectType;
        }


        public Type ObjectType { get; }

        public Type ComplexObjectType { get; }
    }
}