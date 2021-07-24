namespace Aquila.Core.Contracts.Environment
{
    /// <summary>
    /// <br /> Интрфейс среды для администрирования
    /// <br />
    /// <br /> Назначение: Для уменьшения связанности компонентов и для повышения гибкости администрирования
    /// <br /> Представляет собой набор команд для администратора сервера приложения
    /// <br /> Обеспечивает:
    /// <br />    * Создание новых, удаление или подключение существующих баз данных
    /// <br />    * Управление настройками баз данных (строки подключения, блокировки и так далее)
    /// <br />    * Остановка/запуск обслуживания баз (экземпляра приложения)
    /// <br />    * Утилиты для обеспеяения сохранности данных (бэкапирование)
    /// </summary>
    public interface IAdminEnvironment : IInitializibleEnvironment<object>
    {
    }
}