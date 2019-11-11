using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Compiler;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.UI.Ast;

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
        /// Стадия генерации UI интерфейса для пользователя на клиенте
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        void StageUI(XCObjectTypeBase type, UINode node);

        /// <summary>
        /// Стадия 0 Формирование класса без структуры
        /// </summary>
        /// <param name="astTree"></param>
        /// <param name="builder"></param>
        void Stage0(ComponentAstBase astTree, ITypeBuilder builder);

        /// <summary>
        /// Стадия 1 формирование внутреннего каркаса класса (Методы + Свойства + Поля + События)
        /// </summary>
        /// <param name="astTree"></param>
        /// <param name="builder"></param>
        void Stage1(ComponentAstBase astTree, ITypeBuilder builder);
    }
}