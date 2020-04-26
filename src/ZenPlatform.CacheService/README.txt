Сервис кэширования

Служит для того, чтобы не тянуть при каждом запросе от пользователя данные из БД, а пользоваться уже готовыми данными.

Работает следующий образом


                           cache                                    cache
                           key:1                                    key:1
        cache is empty     value: "object"  cache not changed       value: "objectUpd"
             v               v                   v                  v                                               T
-------------|---------------|-------------------|------------------|--------------------------------------------------> 
            user1        lookup to the         user2              user1
      try get the entity    database        try get the entity  apply changes
            key:1                                key:1          to database
        value: "object"                                         and apply changes
                                                                    to cache
                                                                value: "objectUpd"