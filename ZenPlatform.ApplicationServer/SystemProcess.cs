﻿using ZenPlatform.Core;
using ZenPlatform.Core.Environment;

namespace ZenPlatform.WorkProcess
{
    /// <summary>
    /// Системный процесс, позволяет обновлять структуру баз данных
    /// Также может обновлять структуру конфигурации
    /// </summary>
    public class SystemProcess
    {
        //TODO: добавить мигрирование. Миграция конфигурации должно быть атомарным

        private SystemEnvironment _env;

        public SystemProcess(StartupConfig config)
        {
            _env = new SystemEnvironment(config);
            _env.Initialize();
        }

        public void Migrate()
        {
            //Передаём управление системной среде
            _env.Migrate();
        }

        /*
         * Серверный процесс отвечает за применение изменений и предоставление интерфейса для этих изменений.
         * Задача разбивается на несколько частей:
         *
         * 1) TODO: Сделать модель дерева конфигураций, которая будет передаваться между клиентом и сервером
         * 2) TODO: Сделать интерфейс для передаваемых UI елементов компонента
         * 3) TODO: Сделать протокол, который позволяет общаться клиенту и серверу
         */
    }
}