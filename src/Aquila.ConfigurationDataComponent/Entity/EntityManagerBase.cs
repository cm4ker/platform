using Aquila.Core.Contracts.Data.Entity;

namespace Aquila.DataComponent.Entity
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