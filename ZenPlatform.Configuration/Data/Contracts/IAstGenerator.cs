using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Configuration.Data.Contracts
{
    /// <summary>
    /// Последовательный механизм для генерации сборки
    /// </summary>
    public interface IPlatformAstGenerator
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
    }
}