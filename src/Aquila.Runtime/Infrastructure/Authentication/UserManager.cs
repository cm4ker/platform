using System;
using System.IO;
using Aquila.Core.Contracts.Authentication;
using Aquila.Data;
using Aquila.Initializer;
using Aquila.Logging;
using Aquila.QueryBuilder;

namespace Aquila.Core.Authentication
{
    /// <summary>
    /// Менеджер работы с пользователем
    /// Создание/Сохранение/Изменение
    /// </summary>
    public class UserManager
    {
        private readonly DataContextManager _dataContextManager;
        private readonly ILogger<UserManager> _logger;

        public UserManager(DataContextManager dataContextManager, ILogger<UserManager> logger)
        {
            _dataContextManager = dataContextManager;
            _logger = logger;
        }

        public IUser Create()
        {
            return new User();
        }

        public void Update(IUser user)
        {
            var cmd = _dataContextManager.GetContext().CreateCommand();

//            var query = new UpdateQueryNode();
//
//            query
//                .From(DatabaseConstantNames.USER_TABLE_NAME)
//                .Where(x => x.Field(DatabaseConstantNames.USER_TABLE_ID_FIELD), "=", x => x.Parameter("p0"))
//                .Set(x => x.Field(DatabaseConstantNames.USER_TABLE_NAME_FIELD), x => x.Parameter("p1"));

            cmd.AddParameterWithValue("p0", user.Id);
            cmd.AddParameterWithValue("p1", user.Name);

            //TODO: разобраться  с видимостью Evironment в сессии
            //cmd.CommandText = _session.Environment.SqlCompiler.Compile(query);
            cmd.ExecuteNonQuery();
        }

        public void Delete(IUser user)
        {
            var cmd = _dataContextManager.GetContext().CreateCommand();

//            var query = new DeleteQueryNode();
//
//            query
//                .From(DatabaseConstantNames.USER_TABLE_NAME)
//                .Where(x => x.Field(DatabaseConstantNames.USER_TABLE_ID_FIELD), "=", x => x.Parameter("p0"));

            cmd.AddParameterWithValue("p0", user.Id);

            //cmd.CommandText = _session.Environment.SqlCompiler.Compile(query);
            cmd.ExecuteNonQuery();
        }

        public IUser Get(Guid id)
        {
            void Gen(QueryMachine m)
            {
                m.bg_query()
                    .m_from()
                    .ld_table(DatabaseConstantNames.USER_TABLE_NAME)
                    .m_where()
                    .ld_column(DatabaseConstantNames.USER_TABLE_ID_FIELD)
                    .ld_param("p0")
                    .eq()
                    .m_select()
                    .ld_column(DatabaseConstantNames.USER_TABLE_NAME_FIELD)
                    .st_query();

//                var query = new SelectQueryNode();
//
//                query
//                    .From(DatabaseConstantNames.USER_TABLE_NAME)
//                    .Where(x => x.Field(DatabaseConstantNames.USER_TABLE_ID_FIELD), "=", x => x.Parameter("p0"))
//                    .Select(DatabaseConstantNames.USER_TABLE_NAME_FIELD);
            }


            var cmd = _dataContextManager.GetContext().CreateCommand(Gen);
            cmd.AddParameterWithValue("p0", id);

            //cmd.CommandText = _session.Environment.SqlCompiler.Compile(query);
            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var user = new User
                {
                    Id = id,
                    Name = reader.GetString(0)
                };

                return user;
            }

            throw new UserNotFoundException();
        }

        public IUser FindUserByName(string name)`   3N
        {
            void Gen(QueryMachine m)
            {
                m.bg_query()
                    .m_from()
                    .ld_table(DatabaseConstantNames.USER_TABLE_NAME)
                    .m_where()
                    .ld_column(DatabaseConstantNames.USER_TABLE_NAME_FIELD)
                    .ld_param("p0")
                    .eq()
                    .m_select()
                    .ld_column(DatabaseConstantNames.USER_TABLE_PASSWORD_FIELD)
                    .ld_column(DatabaseConstantNames.USER_TABLE_NAME_FIELD)
                    .ld_column(DatabaseConstantNames.USER_TABLE_ID_FIELD)
                    .st_query();
            }

            using (var cmd = _dataContextManager.GetContext().CreateCommand(Gen))
            {
                cmd.AddParameterWithValue("p0", name);

                _logger.Trace(cmd.CommandText);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var user = new User
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                        };

                        return user;
                    }
                }
            }

            throw new UserNotFoundException();
        }

        /// <summary>
        /// Аутентификация по пользователю и паролю
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="password">Пароль(plaintext)</param>
        /// <returns></returns>
        public bool Authenticate(string userName, string password)
        {
            void Gen(QueryMachine m)
            {
                m.bg_query()
                    .m_from()
                    .ld_table(DatabaseConstantNames.USER_TABLE_NAME)
                    .m_where()
                    .ld_column(DatabaseConstantNames.USER_TABLE_NAME_FIELD)
                    .ld_param("p0")
                    .eq()
                    .m_select()
                    .ld_column(DatabaseConstantNames.USER_TABLE_PASSWORD_FIELD)
                    .st_query();
            }


            using (var cmd = _dataContextManager.GetContext().CreateCommand(Gen))
            {
                _logger.Trace(cmd.CommandText);
                _logger.Trace($"Credentials= {userName} : {password}");

                cmd.AddParameterWithValue("p0", userName);

                using (StreamReader reader = new StreamReader(new MemoryStream((byte[])cmd.ExecuteScalar())))
                {
                    var pass = reader.ReadToEnd();

                    return pass.Equals(password);
                }
            }
        }
    }
}