using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Compiler;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Configuration.Data.Contracts
{
    /// <summary>
    /// Последовательный механизм для генерации сборки
    /// </summary>
    public interface IPlatformGenerator
    {
        /// <summary>
        ///  Генерация серверного кода
        /// </summary>
        /// <param name="type">Тип</param>
        /// <param name="root">Корень проекта</param>
        void StageServer(XCObjectTypeBase type, Root root);

        /// <summary>
        /// Генерация клиентского кода
        /// </summary>
        /// <param name="type">Тип</param>
        /// <param name="root">Корень проекта</param>
        void StageClient(XCObjectTypeBase type, Root root);

        /// <summary>
        /// Стадия 0 Формирование класса без структуры
        /// </summary>
        /// <param name="builder"></param>
        void Stage0(ComponentAstBase astTree, ITypeBuilder builder);

        /// <summary>
        /// Стадия 1 формирование внутреннего каркаса класса (Методы + Свойства + Поля + События)
        /// </summary>
        /// <param name="builder"></param>
        void Stage1(ComponentAstBase astTree, ITypeBuilder builder);
    }
}