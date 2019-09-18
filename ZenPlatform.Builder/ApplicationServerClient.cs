using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Cli
{
    /*
     * Для того, чтобы можно было управлять сервером, необходим клиент, который это будет делать
     *
     * Ссписок доступных команд
     * + disconnect - отключиться
     * 
     * + connect [host] [port] - подключиться, после подключения доступен весь основной набор комманд
     *
     * + db
     *     create [dbName] [connectionString]
     *     delete [dbId]
     *     attach [connectionString]
     *     detach [dbId]
     *     backup [dbId]
     *     list
     * 
     * + kill [dbId] [userId]
     *
     * + instance
     *     start
     *     stop - мягкая остановка
     *     kill - принудительная остановка
     *     restart
     *     list
     *     + config
     *         set [paramName] [value]
     *         list
     *
     * + server
     *     stop
     *     restart
     *     start
     *     + config
     *           set [paramName] [value]
     *           list
     *     update [toVersion] - обновить компоненты приложения до версии, по умолчанию максимальная
     *
     * + client 
     *     connect [host] [port] [dbName] [userName] [password] [mode (client|configuration)] - использовать консольный клиент для подключения к прикладному решению
     *     exec [command] - выполнить комманду в прикладном решении (нужно реализовать интерфейс комманд)
     *     cui - запустить консольное отображение
     *     ui - запустить обычный клиент
     *     disconnect
     *     update - обновить базу данных
     *     deploy [configurationFile]
     *     + configuration
     *           tree
     *           set [node} - установить текущий узел
     *           cmd list - получить список команд текущего узла
     *           exec [cmd] - выполнить команду в текущем узле
     */
    
    public class ApplicationServerClient
    {
        public ApplicationServerClient()
        {
            
        }
    }
}