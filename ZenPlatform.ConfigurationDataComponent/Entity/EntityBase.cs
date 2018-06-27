using System.ComponentModel;
using System.Runtime.CompilerServices;
using ZenPlatform.Contracts.Entity;
using ZenPlatform.Core;

namespace ZenPlatform.DataComponent.Entity
{
    /// <summary>
    /// Базовая сущность для объявляемого типа
    /// </summary>
    public abstract class EntityBase : IEntity, INotifyPropertyChanged
    {
        public EntityBase(Session session)
        {
            Session = session;
        }

        /// <summary>
        /// Текущая сессия
        /// </summary>
        protected Session Session { get; }

        /// <summary>
        /// Окружение
        /// </summary>
        protected PlatformEnvironment Environment => Session.Environment;


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}