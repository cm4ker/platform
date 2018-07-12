using System.ComponentModel;
using System.Runtime.CompilerServices;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Core;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Sessions;

namespace ZenPlatform.DataComponent.Entity
{
    /// <summary>
    /// Базовая сущность для объявляемого типа
    /// </summary>
    public abstract class EntityBase : IEntity, INotifyPropertyChanged
    {
        protected EntityBase(UserSession session)
        {
            Session = session;
        }

        /// <summary>
        /// Текущая сессия
        /// </summary>
        protected UserSession Session { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}