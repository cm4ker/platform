using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ZenPlatform.Core;
using ZenPlatform.DataComponent;
using ZenPlatform.DataComponent.Entity;

namespace ZenPlatform.DocumentComponent
{
    public abstract class DocumentEntity : EntityBase, INotifyPropertyChanged
    {
        protected DocumentEntity(Session session)
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

        /// <summary>
        /// Версия объекта, присваивается менеджером при загрузке объекта
        /// При этом, при сохранении, если версия отличается, в таком случае объект становится не пригодным для записи
        /// </summary>
        public object Version { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}