using Microsoft.CodeAnalysis.Diagnostics;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.UIBuilder.Interface;

namespace ZenPlatform.EntityComponent.UIGenerations
{
    /// <summary>
    /// Класс для генерации формы документа
    /// </summary>
    /// <returns></returns>
    public class InterfaceGenerator
    {
        /// <summary>
        /// Сгенерировать форму для редактирования элемента
        /// </summary>
        /// <param name="conf">Конфигурация</param>
        /// <param name="mode">Режим генерации формы, если форма сгенерирована только для чтения, автоматически будут выключены все элементы</param>
        /// <returns></returns>
        public UINode Generate(XCSingleEntity conf, InterfaceGeneratorMode mode = InterfaceGeneratorMode.Edit)
        {
            UIFactory f = UIFactory.Get();

            var window = f.Window();

            var group = f.Group();

            //TODO: Необходимо на разные типы подбирать разные контролы
            foreach (var props in conf.Properties)
            {
                group.With(x => x.TextBox());
            }

            window.With(group);

            return window;
        }
    }


    public enum InterfaceGeneratorMode
    {
        ReadOnly,
        Edit
    }
}