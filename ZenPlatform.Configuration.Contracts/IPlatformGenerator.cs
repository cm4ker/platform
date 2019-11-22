using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Configuration.Contracts
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
        void StageServer(IXCObjectType type, Node root);

        /// <summary>
        /// Генерация клиентского кода
        /// </summary>
        /// <param name="type">Тип</param>
        /// <param name="root">Корень проекта</param>
        void StageClient(IXCObjectType type, Node root);

        /// <summary>
        /// Стадия генерации UI интерфейса для пользователя на клиенте
        /// </summary>
        /// <param name="type"></param>
        /// <param name="uiNode"></param>
        void StageUI(IXCObjectType type, Node uiNode);

        /// <summary>
        /// Стадия 0 Формирование класса без структуры
        /// </summary>
        /// <param name="astTree"></param>
        /// <param name="builder"></param>
        void Stage0(Node astTree, ITypeBuilder builder);

        /// <summary>
        /// Стадия 1 формирование внутреннего каркаса класса (Методы + Свойства + Поля + События)
        /// </summary>
        /// <param name="astTree"></param>
        /// <param name="builder"></param>
        void Stage1(Node astTree, ITypeBuilder builder);

        /// <summary>
        /// Инфраструктурная стадия
        /// </summary>
        /// <param name="builder"></param>
        void StageInfrastructure(IAssemblyBuilder builder);
    }
}