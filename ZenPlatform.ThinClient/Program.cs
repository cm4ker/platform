using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Logging.Serilog;
using Dock.Model;
using Dock.Serializer;
using ZenPlatform.ThinClient.Infrastructure;
using ZenPlatform.ThinClient.TCP;
using ZenPlatform.ThinClient.ViewModels;
using ZenPlatform.ThinClient.Views;
using System.Net;
using ZenPlatform.ServerClientShared.RPC;

namespace ZenPlatform.ThinClient
{
    /*
     * Описание клиентской части
     * (*) Необходимо чтобы была поддержка веба и десктопа
     * Т.е. необходим некий интерфейс, который прозволит проецировать объект и туда и сюда
     * 
     * Для примера:
     * У нас есть объект с тремя текстовыми полями
     *      Текст1, 
     *      Текст2, 
     *      Текст3
     * Пример задачи: расположить текстовые поля в одну колонку, и чтобы они отображались везде одинаково (Web, Desktop)
     * 1) Необходим унифецированный интерфейс отрисовки, который позволит всё это дело строить.
     *      Набор команд
     *          1) ДобавьКонтейнер
     *          2) ДобавьМеню
     *          3) ДобавьПоле
     *          4) ДобавьЧтоТоЕщё
     *          ....
     *          
     *      С помощью этого набора команд, мы посылаем провайдеру который на месте имплементирует интерпретацию этих комманд.
     *      Как это будет всё работать. Для декстопа:
     *      
     *      Desktop Application (.net core) -> (*) -> Network -> ConnectionServer -> BalanceServer -> Cluster -> ApplicationServer -> Database
     *      
     *      Для вэба же необходимо реализовывать промежуточный слой, который будет держаться на ASP.NET приложении:
     *      
     *      Browser -> Asp.Net MVC CORE (webforms) -> (*) -> Network -> ConnectionServer -> BalanceServer -> Cluster -> ApplicationServer -> Database
     *      
     *      Видно что архитектура не меняется, подменивается лишь клиентская часть. Поэтому нужен какой-то механизм для постройки приложений либо какой-то унифицированный интерфейс который необходимо реализовать для каждого приложения.
     *      
     */

    /*
     * Первоначальная задача. Сделать UI, который получает с сервера какой-то объект. Генерирует на основании объекта форму
     * делает биндинги на свойства и показывает форму пользователю
     * 
     * Для этого необходимо 
     * 1) Разработать протокол, на основании которого будет передаваться форма
     * 2) Сделать изменениия в КОМПОНЕНТЕ для генерации формы по умолчанию (никаких изменений в конфигурации делать не нужно)
     * 
     * 
     */

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Bootstrapper.Init();
            BuildAvaloniaApp().Start<MainWindow>(IoC.Resolve<IMainWindow>);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
