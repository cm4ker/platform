using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Settings;

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
        public static Assembly ClientMainAssembly { get; set; }

        static void Main(string[] args)
        {
            var clientServices = Initializer.GetClientService();
            var platformClient = clientServices.GetRequiredService<ClientPlatformContext>();
            platformClient.Connect(new DatabaseConnectionSettings()
            {
                Address = "127.0.0.1:12345",
                Database = "Library"
            });
            //need check connection

            platformClient.Login("admin", "admin");

            ClientMainAssembly = platformClient.LoadMainAssembly();
        }
    }


    public class TestClientAssemblyManager : IClientAssemblyManager
    {
        private IAssembly _assembly;

        public TestClientAssemblyManager(IAssembly assembly)
        {
            _assembly = assembly;
        }

        public Stream GetAssembly(string name)
        {
            if (_assembly != null && _assembly.Name == name)
            {
                var stream = new MemoryStream();
                _assembly.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }

            return new MemoryStream();
        }

        public void UpdateAssemblies()
        {
        }
    }
}