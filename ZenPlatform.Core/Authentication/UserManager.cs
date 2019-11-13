using System;
using System.IO;
using ZenPlatform.Core.Helpers;
using ZenPlatform.Initializer;
using ZenPlatform.Data;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Authentication
{
    /// <summary>
    /// Менеджер работы с пользователем
    /// Создание/Сохранение/Изменение
    /// </summary>
    public class UserManager : IUserManager
    {
        /*
        private readonly ISession _session;

        public UserManager(ISession session)
        {
            if (session is SystemSession sys)
            {
                _session = sys;
            }
            else if (session is UserSession uses)
            {
                if (!uses.User.Roles.Any(x => x.Rights.Any(r =>
                {
                    if (r is SystemRight sysr && sysr.IsDataAdministrator)
                        return true;
                    return false;
                })))
                {
                    throw new NotAuthorizedException();
                }

                _session = uses;
            }
        }
        */

        private readonly IDataContextManager _dataContextManager;

        public UserManager(IDataContextManager dataContextManager)
        {
            _dataContextManager = dataContextManager;
        }

        public IPlatformUser Create()
        {
            return new PlatformUser();
        }

        public void Update(IPlatformUser user)
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

        public void Delete(IPlatformUser user)
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
                m.m_from()
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
                var user = new PlatformUser
                {
                    Id = id,
                    Name = reader.GetString(0)
                };

                return user;
            }

            throw new UserNotFoundException();
        }

        public IUser FindUserByName(string name)
        {
            void Gen(QueryMachine m)
            {
                m.m_from()
                    .ld_table(DatabaseConstantNames.USER_TABLE_NAME)
                    .m_where()
                    .ld_column(DatabaseConstantNames.USER_TABLE_ID_FIELD)
                    .ld_param("p0")
                    .eq()
                    .m_select()
                    .ld_column(DatabaseConstantNames.USER_TABLE_ID_FIELD)
                    .ld_column(DatabaseConstantNames.USER_TABLE_NAME_FIELD)
                    .ld_column(DatabaseConstantNames.USER_TABLE_PASSWORD_FIELD)
                    .st_query();

//                var query = new SelectQueryNode();
//
//                query
//                    .From(DatabaseConstantNames.USER_TABLE_NAME)
//                    .Where(x => x.Field(DatabaseConstantNames.USER_TABLE_NAME_FIELD), "=", x => x.Parameter("p0"))
//                    .Select(DatabaseConstantNames.USER_TABLE_ID_FIELD)
//                    .Select(DatabaseConstantNames.USER_TABLE_NAME_FIELD)
//                    .Select(DatabaseConstantNames.USER_TABLE_PASSWORD_FIELD);
            }

            using (var cmd = _dataContextManager.GetContext().CreateCommand(Gen))
            {
                cmd.AddParameterWithValue("p0", name);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var user = new PlatformUser
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
                m.m_from()
                    .ld_table(DatabaseConstantNames.USER_TABLE_NAME)
                    .m_where()
                    .ld_column(DatabaseConstantNames.USER_TABLE_ID_FIELD)
                    .ld_param("p0")
                    .eq()
                    .m_select()
                    .ld_column(DatabaseConstantNames.USER_TABLE_PASSWORD_FIELD)
                    .st_query();

//                var query = new SelectQueryNode();
//
//                query
//                    .From(DatabaseConstantNames.USER_TABLE_NAME)
//                    .Where(x => x.Field(DatabaseConstantNames.USER_TABLE_NAME_FIELD), "=", x => x.Parameter("p0"))
//                    .Select(DatabaseConstantNames.USER_TABLE_PASSWORD_FIELD);
            }


            using (var cmd = _dataContextManager.GetContext().CreateCommand(Gen))
            {
                cmd.AddParameterWithValue("p0", userName);

                using (StreamReader reader = new StreamReader(new MemoryStream((byte[]) cmd.ExecuteScalar())))
                {
                    var pass = reader.ReadToEnd();

                    return pass.Equals(password);
                }
            }
        }
    }
}