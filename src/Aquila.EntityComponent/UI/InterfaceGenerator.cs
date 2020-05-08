using System.Linq;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis.Diagnostics;
using Aquila.Configuration.Structure.Data.Types.Primitive;
using Aquila.EntityComponent.Configuration;


namespace Aquila.EntityComponent.UIGenerations
{
    /// <summary>
    /// Класс для генерации формы документа
    /// </summary>
    /// <returns></returns>
    public class InterfaceGenerator
    {
        //TODO: Необходимо переписать всю генерацию кода через PIDL

        /*
         * PIDL Это промежуточный языык, который позволит обеспечить кастомное отображение формы.
         * Если мы хотим сгенерировать форму, то мы должны использовать именно язык разметки PIDL.
         *
         * В случае удаления реквизита с формы для него не должно генерироваться никаких контролов
         */

        /// <summary>
        /// Сгенерировать форму для редактирования элемента
        /// </summary>
        /// <param name="conf">Конфигурация</param>
        /// <param name="mode">Режим генерации формы, если форма сгенерирована только для чтения, автоматически будут выключены все элементы</param>
        /// <returns></returns>
//        public UINode Generate(XCSingleEntity conf, InterfaceGeneratorMode mode = InterfaceGeneratorMode.Edit)
//        {
//            UIFactory f = UIFactory.Get();
//
//            var window = f.Window();
//
//            var group = f.Group();
//
//            //TODO: Необходимо на разные типы подбирать разные контролы
//            foreach (var props in conf.Properties)
//            {
//
//                //Если количество типов 1, тогда мы смотрим если он примитивный,
//                //то рендерим соответствующий контрол, если нет, то нужно рендерить контрол на чужую форму из компонента
//                if (props.Types.Count == 1)
//                {
//                    var foreigFieldType = props.Types.First();
//
//                    //Примитивный тип, значит просто рендерим обычный контрол
//                    if (foreigFieldType is XCPremitiveType)
//                    {
//                    }
//                }
//
//                group.With(x => x.TextBox());
//            }
//
//            window.With(group);
//
//            return window;
//        }
    }


    public enum InterfaceGeneratorMode
    {
        ReadOnly,
        Edit
    }
}