using ZenPlatform.Configuration.Data.Contracts.Entity;

namespace ZenPlatform.DataComponent.Entity
{
    /// <summary>
    /// Менеджер - это НАБОР ЛОГИКИ для манипулирования сущностями (Entity)
    /// Менеджер только лишь инкапсулирует это в себе.
    /// </summary>
    public abstract class EntityManager : IEntityManager
    {
        protected EntityManager()
        {
        }
    }
}