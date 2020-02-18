using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Configuration.Contracts.Data
{
    /// <summary>
    /// Последовательный механизм для генерации сборки
    /// </summary>
    public interface IPlatformGenerator
    {
        /// <summary>
        ///  Генерация серверного кода
        /// </summary>
        /// <param name="ipType">Тип</param>
        /// <param name="root">Корень проекта</param>
        void StageServer(IPType ipType, Node root, SqlDatabaseType dbType);

        /// <summary>
        /// Генерация клиентского кода
        /// </summary>
        /// <param name="ipType">Тип</param>
        /// <param name="root">Корень проекта</param>
        void StageClient(IPType ipType, Node root, SqlDatabaseType dbType);

        /// <summary>
        /// Стадия генерации UI интерфейса для пользователя на клиенте
        /// </summary>
        /// <param name="ipType"></param>
        /// <param name="uiNode"></param>
        void StageUI(IPType ipType, Node uiNode);

        /// <summary>
        /// Стадия генерации глобального пространства
        /// </summary>
        /// <param name="manager"></param>
        void StageGlobalVar(IGlobalVarManager manager);

        /// <summary>
        /// Стадия формирования класса
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        ITypeBuilder Stage0(IAssemblyBuilder asm, Node task);

        /// <summary>
        /// Стадия 1 формирование внутреннего каркаса класса (Методы + Свойства + Поля + События)
        /// </summary>
        /// <param name="astTree"></param>
        /// <param name="builder"></param>
        /// <param name="dbType"></param>
        /// <param name="mode"></param>
        void Stage1(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode);

        /// <summary>
        /// Стадия 2 формирование реализаций методов и свойств
        /// </summary>
        /// <param name="astTree"></param>
        /// <param name="builder"></param>
        /// <param name="dbType"></param>
        /// <param name="mode"></param>
        void Stage2(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode);

        /// <summary>
        /// Инфраструктурная стадия
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="dbType"></param>
        /// <param name="mode"></param>
        void StageInfrastructure(IAssemblyBuilder builder, SqlDatabaseType dbType, CompilationMode mode);
    }
}