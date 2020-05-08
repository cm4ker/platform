# Aquila


В современном мире очень важно время и минимизация издержек на разрабтку. Существует достаточно много различных технологий, благодаря которым строятся современные высокопроизводительные системы. 

В платформу заложены следующие принципы:

1. Наследование

2. Инкапсуляция*

3. Полиморфизм* 

4. Предоставить разработчику богатый SQL-подобный синтаксис для выборки данных из базы

5. Обеспечить максимальную производительность встроенного языка для решения разнообразного типа задач

6. Открытый исходный код

5. Расширяемость

Общая карта проекта:
![](images/img1.png)

# Конфигурация 

## Структура

Конфигурация состоит из следующих разделов:

1. **Данные** (Data system). Подсистема для работы с данными. А именно выполнение CRUD(CREATE\READ\UPDATE\DELETE) операций над ними.
2. **Доступ к данным** (Role system). Подсистема для разграничения доступ к данным
3. **Отображение** (Interface system). Подсистема обеспечивающая доступ к данным определённого типа через какой-либо интерфейс (web\desktop client)
4. **Отчетность** (Report system). Подсистема обеспечивающая вывод какого-либа набора данных в определённом формате с возможностью сохранить\распечатать эти данные
5. **Произвольная логика** (Modules system). Произвольная логика, может быть независима от данных.
6. **Регламентные задания** (Schedule system). Переодические задания
7. **Локализация** (Language system). Доступные языковые пакеты системы
8. **Интеграция данных** (Integration system). Подсистема интеграции с другим ИС.
## Данные 

### (Root -> Data -> Components -> Component)

В данных описываются все компоненты, которые подключены к конфигурации. 

Пример:
```xml
<Component>
    <File Path="./Components/DocumentComponent.dll"/>
</Component>
```

При загрузке конфигурации dll по этому пути будет загружена с помощью метода ```Assembly.LoadFromFile(string)```
Если вы хотите подробней узнать о механизме загрузки конфигурации в платформу перейдите в этот раздел (TODO: добавить ссылку на раздел)

### (Root -> Data -> IncludedFiles -> File)

Также в данных опысываются файлы, так называемые исполняемые модули для платформы. Они подключаются следующим образом:

```xml
<IncludedFiles>
            <!--  Файл проекта, где ComponentId - это уникальный идентификатор компонента, с помощью
                  него будет осуществляться загрузка данного файла, если по какой-то из причин компонент не был найден,
                  то бует выдано исключение InvalidPlatformComponent -->
            <File Path="./Data/Documents/ВыдачаКниги.xml" ComponentId="230c6759-ae4e-408f-94b9-798749333f07"/>
</IncludedFiles>
```

В каждом файле описан объект согласно специфике компонента, к которому он привязан. Этот файл будет загружаться непосредственно компонентом и регистрироваться в общем списке доступных типов данных в платформе.

## Пример

Пример конфигурации в xml виде вы можете найти здесь (TODO: Добавить ссылку на пример конфигурации)

# Платформа

## Архитектура
Платформа состоит из следующих частей:


## **Стэковая машина компиляции запросов**

### Вступление


## Описание и примеры
На начальной стадии есть следующая поддержка: (MSSQL, PostgreSQL(не полная поддержка)) 

![](images/img2.png)


# **Серверное приложение**

Серверное приложение, как и говорилось ранее, отвечает за обработку запросов клиента. Помимо этого серверное приложение обеспечивает ещё один целостный уровень изоляции данных. Здесь можно блокировать данные, также серверная часть полностью обеспечивает RLS(Row-Level Security).

Серверная часть имеет внутри себя несколько компонентов. 

1) Сервер системных процессов
2) Сервер рабочих процессов

Сервер системных процессов обеспечивает обновление различных компонентов системы, в то время как рабочий процесс обеспечивает работу с данными. Основная особенность этого механизма - __***полное динамическое обновление системы***__ в режиме реального времени. Это достигается за счет блокировки только обнолвяемых объектов в рабочем процессе, после обновления, блокировка отпускается и все существующие соединения начинают работась совым рабочим процессом, а старый рабочий процесс убивается. Более подробный механизм обновления конфигурации описан в разделе ([TODO: написать раздел обновления конфигурации]())  

Основное отличие рабочего процесса от системного - невозможность рабочего процесс менять структуру данных.

![](images/img3.png)


# Клиентское приложеине

Клиентское приложеине - неотьемлемая часть программы учавствующая в жизненном цикле данных. Комплекс явлется непосредственным источником и манипулятором данных. Клиенское приложение выполняет роль тонкого клиента. Т.е. вся логика по обработке данных находится на сервере. На клиенте есть лишь минимальный набор функций, связанных с трансформацией данных представления, подсчёт итогов и т.д.

![](interface.svg)

# Правила именования колонок

Ниже представлен алгоритм, как будет колонка разворачиваться в базу данных:

1) Компонент обязан указать префикс, который будет указываться при генерации имени колонки. В примере это будет "Fld".

2) У каждого типа есть поле ```uint Id``` - это уникальный идентификатор в разрезе базы данных.

Итого получаем следующий расклад
DatabaseColumnName = Fld_035|Fld_035_TypeId|Fld_035_TypeRef|Fld_035_Binary|Fld_035_Guid|Fld_035_Int|Fld_035_DateTime|Fld_035_String

 Когда Types.Count() == 1 и Types[0] is XCPrimitiveType
 В таком случае выделяется единственная колонка колонка для хранения
      Guid || binary || bool || int || datetime
 Когда Types.Count() == 1 и Types[0] is XCObjectType и Type.IsAabstract в таком случае выделяются две колонки
      IntTypeId, GuidRef
 Когда Types.Count() == 1 и Types[0] is XCObjectType и !Type.IsAabstract в таком случае выделяется две или одна колонка.
 Примечание. Всё зависит от того, есть ли унаследованные объекты от текущего объекта
      GuidRef \ IntTypeId, GuidRef
 Когда Types.Count() > 1 и все Types is XCPrimitiveType
      В таком случае на каждый тип отводится своя колонка. Биндинг должен осуществляться таким
      не хитрым мапированием: Свойство, Тип -> Колонка

## Таблица сопоставления

|УИД Объекта(Guid)| Локальный ИД базы(Int)  |
|---|---|
| 196b5c33-412d-4ca1-b4e5-67a72f5a10d7  | 1  |
|69a9f0ac-a7dd-46a4-8fb5-a8b3da86a0e2   | 2  |
|71181374-b294-44e6-bf13-83caff334af2   | 3  |
|7d16af5b-8c7a-475f-a6c2-db7151a0baf4   | 4  |
|...|...|


# IDE 


## Сервер конфигурации
Сервер конфигурации это промежуточный слой абстракции для работы с конфигурацией.
Этот промежуточный слой содержит модель, которая передаётся в ПО, для отображения.

Эта прослойка работает как почтовое отделение. 

Например: к нам приходит сообщение, о том, что необходимо распахнуть ветку дерева конфигурации.
У нас, в сервере конфигурации есть модель текущего сеанса. Мы распахиваем ветку. и возвращаем
модель-разницу обратно, и благополучно применяем её на клиенте.

Более близко к теме

Допустим, у нас сейчас такое состояние:

    - Корень
        - Данные
            + Документы
            + Справоники
            + Регистры
        + Отчеты
        + Интерфейс
        + API
        + Модули

Мы распахиваем пункт "Документы". На сервер конфигурации летит 
сообщение: "РаспахнутьЭлемент(ИдЭлемента, ИдСессии)". После этого, на сервере происходит разворот
этого элемента в результате чего мы имеем следующую модель:

    - Корень
        - Данные
            - Документы
                + ПриходнаяНакладная
                + РасходнаяНакладная
                + Перемещение
                + Списание
            + Справоники
            + Регистры
        + Отчеты
        + Интерфейс
        + API
        + Модули

В клиентскую сторону передаётся лишь следующая информация:
    
    + ПриходнаяНакладная
    + РасходнаяНакладная
    + Перемещение
    + Списание

## Плюсы и минусы

    + Имплементация на 1 раз. Вся работа с моделью осуществляется через специальную модель команд. Т.е. нет никакого недопонимания в имплементации через разные платформы (VS, AvalonStudio, VS Code... и так далее )
    - Сложность реализации. Необходимо продумыввать и контролировать ещё один слой абстракции для работы с конфигурацией, хотя... Это всё такие, сомнительные трудности
    + Лёгкая интеграция куда угодно. Необходимо только лишь имплементировать команды
    + Расширяемость. На разных платформах (предположим) могут быть доступны разные функции.
    + 

<img src="images/img_conf_server_scheme.png" align="center"/>

### Поддерживаемые команды:
 
 * Редактировать настройки конфигурации

 * Добавить компонент
 * Удалить компонент
 * Редактировать настройки компонента
 
 * Добавить объект компонента
 * Редактировать объект компонента
 * Удалить объект компонента
 
 * Редактировать интерфейс объекта компонента
 
 * Получить дерево объектов конфигурации
 * Распахнуть ветку дерева объектов конфигурации
 * Свернуть ветку дерева конфигурации
 * Добавить пользователя
 * Редактировать пользователя
 * Удалить пользователя
 * Просмотреть журнал событий
 * Добавить роль
 * Редактировать роль
 * Удалить роль
 * ?

<span style="color:red; font-weight: bold;">Внимание! До меня неожиданно дошло что сервер управления конфигурацией необходим, можно сказать, 
спонтанно, и это решение либо чертовски офигенное, либо чертовски бредовое. Я считаю, что это необходимо реализовать для 
прототипа.</span>

## Подсистема обработки сообщений

### Вступление
Для того, чтобы обеспечить коммуникацию между компонентами приложения представляю компонент обработки сообщений, который будет встроен непосредственно в ядро платформы.
Для начала представим такую картинку:

![](./images/img_message_routing.png)

Здесь представлена собственно сама затея. Ядро платформы умеет дополнительно маршрутизировать сообщения от компонента к компоненту.
Каждый компонент системы **имеет право** на регистрацию своего обработчика сообщений. На текущем этапе происходит следующее:
1) Компонент регистрирует обработчик событий в серверном ядре.
2) Серверное ядро при приёме очередного сообщения отправляет его на все обработчики событий(обработчики событий сейчас обрабатываются синхронно, т.е. по порядку). Каждый обработчик сам решает нужно ли обрабатывать ему это сообщение или нет.
3) Если необходимо, обработчик может имплементировать обратную отсылку сообщений к вызывающей стороне.

## Подсистема кэширования

Для того, чтобы обеспечить ускорение доступа к частоиспользуемым объектам в платформе предусмотрен сервер кэширования

В качестве backend используется популярный сервер [redis](https://github.com/ServiceStack/redis-windows).

Инструкция по развертыванию находится [здесь](https://github.com/ServiceStack/redis-windows#option-3-running-microsofts-native-port-of-redis)


## Комманды

<i> Пользователь сам должен выбирать куда ему тыкать </i>

Ко всем кнопкам платформы должны быть привязаны какие-то действия - <span style="color: yellow"> Команды </span>.

Мы в любой момент времени, если у нас доступен побъект можем получить список комманд, которые мы можем совершать над ним

И я не говорю про менеджер. Допустим, мы можем делать следующее:

    1) Открыть список документов
    2) Создать документ
    3) Удалить документ
    4) Открыть документ
    5) Провести документ
    6) Распровести документ

И эти все - это комманды, позади которых стоят уже конкретные алгоритмы платформы

Комманды должны быть описыны примерно следующим образом:

```csharp
public class Command
{
    // Предпределенная ли это команда (не доступна для редактирования)
    bool Predefined { get; }

    // Текстовое представление комманды
    string Name { get; set; }

    // Явное отображение комманды в интерфейсе
    string Display  { get; set; }

    // Какую процедуру выполняет комманда
    string Handler { get; set; }

    // Аргументы команды
    List<object> Arguments { get; }
}
```