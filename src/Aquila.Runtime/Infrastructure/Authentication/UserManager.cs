using System;
using System.IO;
using Aquila.Core.Contracts.Authentication;
using Aquila.Data;
using Aquila.Initializer;
using Aquila.Logging;
using Aquila.QueryBuilder;

namespace Aquila.Core.Authentication
{
    public class UserManager
    {
        private readonly DataContextManager _dataContextManager;
        private readonly ILogger<UserManager> _logger;

        public UserManager(DataContextManager dataContextManager, ILogger<UserManager> logger)
        {
            _dataContextManager = dataContextManager;
            _logger = logger;
        }

        public AqUser Create()
        {
            return new AqUser();
        }

        public void Update(AqUser user)
        {
        }

        public void Delete(AqUser user)
        {
        }

        public AqUser Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public AqUser FindUserByName(string name)
        {
            throw new NotImplementedException();
        }

        public bool Authenticate(string userName, string password)
        {
            throw new NotImplementedException();
        }
    }
}