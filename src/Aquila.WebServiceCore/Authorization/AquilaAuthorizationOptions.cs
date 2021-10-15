using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Aquila.WebServiceCore
{
    public class AquilaAuthorizationOptions
    {
        private List<object> _list = new();

        public void AddAuthorizeData(string policy = null, string roles = null, string schemes = null)
        {
            _list.Add(new AuthorizeAttribute
            {
                Policy = policy,
                Roles = roles,
                AuthenticationSchemes = schemes
            });
        }

        public void AllowAnonymous()
        {
            _list.Add(new AllowAnonymousAttribute());
        }

        public IReadOnlyList<T> GetData<T>()
            => _list.OfType<T>().ToImmutableArray();
    }
}