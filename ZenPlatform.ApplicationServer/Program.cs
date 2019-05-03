using System;
using System.Linq;
using Mono.Cecil;


namespace ZenPlatform.WorkProcess
{
    public static class Program
    {
        public static void Main(params string[] args)
        {
            var ad = AssemblyDefinition.CreateAssembly(
                new AssemblyNameDefinition("Debug", new Version(1, 0)), "Debug", ModuleKind.Dll);
            var md = ad.MainModule;

            
            var asmNr = (AssemblyNameReference) md.TypeSystem.CoreLibrary;
            var asmNd = new AssemblyNameDefinition(asmNr.Name, asmNr.Version);

            
            
            var asm = (new DefaultAssemblyResolver()).Resolve(asmNd);
            ExportedType a = asm.MainModule.ExportedTypes.FirstOrDefault(x => x.Name == "Array");
            
            //md.ImportReference(a);
            //var correct = md.TypeSystem.Int32.Resolve(); // System.Int32
            var incorrect = new ArrayType(md.TypeSystem.Int32).Resolve(); // System.Int32
            
        }
    }

    public class Test
    {
        private int Value { get; set; } = 235;
    }

    /// <summary>
    /// Рабочий процесс. Обеспечивает связь между сервером и соответственно функциями платформы.
    /// Сервер содержит в себе настройки, которые передаёт рабочему процессу.
    /// Одновременно могут работать несколько рабочих процессов.
    /// Рабочий процесс может лишь манипулировать данными и иметь доступ к конфигурации только на чтение.
    /// </summary>
    /*  public class WorkProcess
      {
          private WorkEnvironment _env;
  
          public WorkProcess(StartupConfig config)
          {
              _env = new WorkEnvironment(config);
          }
  
          public void Start()
          {
              _env.Initialize();
          }
  
          public void Stop()
          {
              //TODO: Выгрузить все ресурсы, потребляемые процессом
          }
  
  
          /// <summary>
          /// Текущее состояние процесса
          /// </summary>
          public string Status { get; set; }
  
  
          /// <summary>
          /// Зарегистрировать соединение, т.е. создать сессию для соединения
          /// </summary>
          public void RegisterConnection(User user)
          {
              //TODO: выполнить проверку контрольной ссумы пользователя, для того, чтобы не получилось подмены
              _env.CreateSession(user);
          }
      }
  
      /*
       * Необходимо сделать несколько протоколов общения
       *
       * Типа всё - это микросервисы.
       *
       * 1) Сервер <-> Рабочий процесс
       * 2) Рабочий процесс <-> Сервер кэша и транзакций
       */
    public class WorkProcessProtocol
    {
        /*
         *Список команд:
         *     1) Получить объект (Ид, Маршрут)
         *     2) Получить список объектов (Маршрут)
         */

        public void ExecuteCommand()
        {
        }

        public void AuthorizeUser()
        {
        }
    }
}