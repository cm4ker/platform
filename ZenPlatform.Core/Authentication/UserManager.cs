using System;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization.Formatters;
using ZenPlatform.Core.Helpers;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Initializer;
using ZenPlatform.QueryBuilder.DML.Delete;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.QueryBuilder.DML.Update;

namespace ZenPlatform.Core.Authentication
{
    /// <summary>
    /// Менеджер работы с пользователем
    /// Создание/Сохранение/Изменение
    /// </summary>
    public class UserManager
    {
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

        public User Create()
        {
            return new User();
        }

        public void Update(User user)
        {
            var context = _session.GetDataContext();

            var cmd = context.CreateCommand();

            var query = new UpdateQueryNode();

            query
                .From(DatabaseConstantNames.USER_TABLE_NAME)
                .Where(x => x.Field(DatabaseConstantNames.USER_TABLE_ID_FIELD), "=", x => x.Parameter("p0"))
                .Set(x => x.Field(DatabaseConstantNames.USER_TABLE_NAME_FIELD), x => x.Parameter("p1"));

            cmd.AddParameterWithValue("p0", user.Id);
            cmd.AddParameterWithValue("p1", user.Name);

            //TODO: разобраться  с видимостью Evironment в сессии
            //cmd.CommandText = _session.Environment.SqlCompiler.Compile(query);
            cmd.ExecuteNonQuery();
        }

        public void Delete(User user)
        {
            var context = _session.GetDataContext();

            var cmd = context.CreateCommand();

            var query = new DeleteQueryNode();

            query
                .From(DatabaseConstantNames.USER_TABLE_NAME)
                .Where(x => x.Field(DatabaseConstantNames.USER_TABLE_ID_FIELD), "=", x => x.Parameter("p0"));

            cmd.AddParameterWithValue("p0", user.Id);

            //cmd.CommandText = _session.Environment.SqlCompiler.Compile(query);
            cmd.ExecuteNonQuery();
        }

        public User Get(Guid id)
        {
            var context = _session.GetDataContext();

            var cmd = context.CreateCommand();

            var query = new SelectQueryNode();

            query
                .From(DatabaseConstantNames.USER_TABLE_NAME)
                .Where(x => x.Field(DatabaseConstantNames.USER_TABLE_ID_FIELD), "=", x => x.Parameter("p0"))
                .Select(DatabaseConstantNames.USER_TABLE_NAME_FIELD);

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

        /// <summary>
        /// Аутентификация по пользователю и паролю
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="password">Пароль(plaintext)</param>
        /// <returns></returns>
        public bool Authenticate(string userName, string password)
        {
            throw new NotAuthorizedException();
        }
    }
}