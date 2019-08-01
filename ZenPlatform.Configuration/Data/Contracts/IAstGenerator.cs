using System.Collections.Generic;
using System.Collections.Immutable;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Configuration.Data.Contracts
{
    /// <summary>
    /// Последовательный механизм для генерации сборки
    /// </summary>
    public interface IPlatformStagedAssemblyGenerator
    {
        /// <summary>
        /// Создаём классы DTO используя уже готовые типы CLR
        /// </summary>
        /// <param name="type"></param>
        /// <param name="builder"></param>
        void Stage0(XCObjectTypeBase type, IAssemblyBuilder builder);

        /// <summary>
        /// Создаём классы элементов.
        /// Необходимо для того, чтобы мы могли создать свойства и методы с возвращаемыми типами
        /// </summary>
        /// <param name="type">Тип платформы</param>
        /// <param name="builder">Билдер сборки</param>
        ITypeBuilder Stage1(XCObjectTypeBase type, IAssemblyBuilder builder);

        /// <summary>
        /// Декарируем классы элементов. Свойства, методы  и так далее
        /// Необходимо для того, чтобы мы могли в коде ссылаться на эти поля
        /// </summary>
        /// <param name="type">Тип платформы</param>
        /// <param name="builder">Билдер типа CLR</param>
        /// <param name="platformTypes">Все пробилженные типы платформы</param>
        /// <param name="asmBuilderd">Билдер сборки</param>
        void Stage2(XCObjectTypeBase type, ITypeBuilder builder,
            ImmutableDictionary<XCObjectTypeBase, IType> platformTypes, IAssemblyBuilder asmBuilderd);

        /// <summary>
        /// Наполенение методов инструкциями
        /// </summary>
        /// <param name="type"></param>
        /// <param name="builder"></param>
        /// <param name="platformTypes"></param>
        /// <param name="asmBuilderd"></param>
        void Stage3(XCObjectTypeBase type, ITypeBuilder builder,
            ImmutableDictionary<XCObjectTypeBase, IType> platformTypes, IAssemblyBuilder asmBuilderd);
    }
}